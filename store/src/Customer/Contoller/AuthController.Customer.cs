using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Neo4j.Driver;

[ApiController]
[Route("customer")]
public class AuthCustomerController : ControllerBase
{
    private readonly IConfiguration configuration;
    private readonly MongoClient client;
    private readonly IDriver driver;
    private readonly Neo4jQuery neo4JQuery;
    private readonly IMongoDatabase db;
    public AuthCustomerController(IConfiguration configuration)
    {
        this.configuration = configuration;
        client = new MongoClient(Environment.GetEnvironmentVariable("MONGODB_URI"));
        db = client.GetDatabase("Store");

        var uri = Environment.GetEnvironmentVariable("URI");
        var user = Environment.GetEnvironmentVariable("Username");
        var password = Environment.GetEnvironmentVariable("Password");

        driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        neo4JQuery = new();
    }

    [Route("signup")]
    [HttpPost]
    public async Task<ActionResult> SignUp([FromBody] CustomerModel newCustomer)
    {
        newCustomer.Password = Argon2.Hash(newCustomer.Password);
        newCustomer.RefreshToken = GenerateRefreshToken();
        newCustomer.RefreshTokenTimeExpire = DateTime.UtcNow.AddDays(7);
        try
        {
            await driver.VerifyConnectivityAsync();
            await using var session = driver.AsyncSession();
            var customerRef = db.GetCollection<CustomerModel>("Customer");

            var filter = Builders<CustomerModel>.Filter.Eq(f => f.Email, newCustomer.Email);
            var isExsist = await customerRef.Find<CustomerModel>(filter).FirstOrDefaultAsync();

            if (isExsist is null)
            {
                await customerRef.InsertOneAsync(newCustomer);
                var query = @"
                CREATE(n:Customer {id:$id})";
                var parameters = new Dictionary<string, object>
                {
                    {"id",newCustomer._id.ToString()},
                };
                var result = await neo4JQuery.ExecuteWriteAsync(session, query, parameters);

                return await TokenWorker(newCustomer);
            }
            else return Conflict("Email is allready taken");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult> Login([FromBody] LoginModel loginModel)
    {
        try
        {
            var customerRef = db.GetCollection<CustomerModel>("Customer");
            var filter = Builders<CustomerModel>.Filter.Eq(f => f.Email, loginModel.Email);
            var loginCustomer = await customerRef.Find<CustomerModel>(filter).FirstOrDefaultAsync();
            if (loginModel != null && Argon2.Verify(loginCustomer.Password, loginModel.Password))
            {
                //Console.WriteLine("Stari refresh token  " + loginCustomer.RefreshToken);
                //var refreshToken = await GenerateAndSaveRefreshTokenAsync(loginModel);
                return await TokenWorker(loginCustomer);
            }
            return BadRequest(new
            {
                message = "Not correct email or password"
            });
        }
        catch (System.Exception)
        {
            throw;
        }
    }
    private string CreateJWT(JwtModel customer)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier,customer.sub),
            new(ClaimTypes.Name,customer.email),
            new(ClaimTypes.Role,customer.role)   //Sve sto treba da bude u jwt tokenu se smesta u Claim
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: configuration.GetValue<string>("AppSettings:Issuer")!,
            audience: configuration.GetValue<string>("AppSettings:Audience")!,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
    private string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }

    private async Task<string> GenerateAndSaveRefreshTokenAsync(LoginModel loginData)
    {
        var refreshToken = GenerateRefreshToken();
        //Console.WriteLine("Novi refreshToken  " + refreshToken);
        //var context = new EditCustomerController();
        await UpdateRefreshTokenAsync(new CustomerModel
        {
            RefreshToken = refreshToken,
            RefreshTokenTimeExpire = DateTime.UtcNow.AddDays(7),
            Email = loginData.Email,
            Password = loginData.Password,
            Role = "",
            Name = "",
            LastName = "",
            PhoneNumber = ""
        });
        return refreshToken;
    }
    private async Task<ActionResult> TokenWorker(CustomerModel customer)
    {
        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        };
        var refreshToken = await GenerateAndSaveRefreshTokenAsync(new LoginModel(customer.Email, customer.Password));
        Response.Cookies.Append("refreshToken", refreshToken.ToString(), options);
        Response.Cookies.Append("customerID", customer._id.ToString(), options);

        return Ok(
            new ResponseTokenModel
            {
                AccessToken = CreateJWT(new JwtModel
                {
                    email = customer.Email,
                    sub = customer._id,
                    role = customer.Role
                }),
                CustomerId = customer._id,
                Role = customer.Role
            }
        );
    }
    [HttpGet]
    [Route("refresh-token")]
    public async Task<ActionResult> ValidateRefreshTokenAsync()
    {
        string refreshToken = Request.Cookies["refreshToken"];
        try
        {
            var customerRef = db.GetCollection<CustomerModel>("Customer");
            var filter = Builders<CustomerModel>.Filter.Eq(f => f.RefreshToken, refreshToken);
            var result = await customerRef.Find(filter).FirstOrDefaultAsync();
            if (result is not null)
            {
                var dateNow = DateTime.UtcNow.ToLocalTime();
                var dateParse = DateTime.Parse(result.RefreshTokenTimeExpire.ToString()).ToLocalTime();
                if (dateNow < dateParse)
                {
                    return Ok(
                        new ResponseTokenModel
                        {
                            AccessToken = CreateJWT(new JwtModel
                            {
                                email = result.Email,
                                sub = result._id,
                                role = result.Role
                            }),
                            CustomerId = result._id,
                            Role = result.Role
                        });
                }
            }
            return BadRequest("Refresh tokeh has expire!"); //ponovno logovanje
        }
        catch (Exception ex)
        {
            return NotFound(ex);
        }
    }
    private async Task<ActionResult> UpdateRefreshTokenAsync(CustomerModel updateCustomer)
    {
        try
        {
            var customerRef = db.GetCollection<CustomerModel>("Customer");

            var update = Builders<CustomerModel>.Update
                .Set(x => x.RefreshToken, updateCustomer.RefreshToken)
                .Set(x => x.RefreshTokenTimeExpire, updateCustomer.RefreshTokenTimeExpire);

            var result = await customerRef.UpdateOneAsync(
                x => x.Email == updateCustomer.Email,
                update
            );
            if (result.ModifiedCount > 0)
                return Ok("Item is successfully updated");
            return BadRequest("Something went wrong");

        }
        catch (Exception ex)
        {
            return NotFound(ex);
        }
    }
    [HttpGet]
    [Route("signout")]
    public void CustomerSignOut()
    {
        string newRefreshToken = "";
        string customer = "";
        var option = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        };
        Response.Cookies.Append("refreshToken", newRefreshToken, option);
        Response.Cookies.Append("customerID", customer, option);
    }
}