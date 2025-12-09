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
    public async Task<ActionResult> SignUp([FromBody] CustomerModel newUser)
    {
        newUser.Password = Argon2.Hash(newUser.Password);
        newUser.RefreshToken = GenerateRefreshToken();
        newUser.RefreshTokenTimeExpire = DateTime.UtcNow.AddDays(7);
        try
        {
            await driver.VerifyConnectivityAsync();
            await using var session = driver.AsyncSession();
            var userRef = db.GetCollection<CustomerModel>("User");

            var filter = Builders<CustomerModel>.Filter.Eq(f => f.Email, newUser.Email);
            var isExsist = await userRef.Find<CustomerModel>(filter).FirstOrDefaultAsync();

            if (isExsist is null)
            {
                await userRef.InsertOneAsync(newUser);
                var query = @"
                CREATE(n:Customer {id:$id})";
                var parameters = new Dictionary<string, object>
                {
                    {"id",newUser._id.ToString()},
                };
                var result = await neo4JQuery.ExecuteWriteAsync(session, query, parameters);

                return await TokenWorker(newUser);
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
            var userRef = db.GetCollection<CustomerModel>("User");
            var filter = Builders<CustomerModel>.Filter.Eq(f => f.Email, loginModel.Email);
            var loginUser = await userRef.Find<CustomerModel>(filter).FirstOrDefaultAsync();
            if (loginModel != null && Argon2.Verify(loginUser.Password, loginModel.Password))
            {
                var refreshToken = await GenerateAndSaveRefreshTokenAsync(loginModel);
                return await TokenWorker(loginUser);
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
    private string CreateJWT(JwtModel user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier,user.sub),
            new(ClaimTypes.Name,user.email),
            new(ClaimTypes.Role,user.role)   //Sve sto treba da bude u jwt tokenu se smesta u Claim
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
        var context = new EditCustomerController();
        await context.EditCustomerAsync(new CustomerModel
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
    private async Task<ActionResult> TokenWorker(CustomerModel user)
    {
        var refreshToken = await GenerateAndSaveRefreshTokenAsync(new LoginModel(user.Email, user.Password));
        Response.Cookies.Append("refreshToken", user.RefreshToken.ToString(), new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });
        return Ok(
            new ResponseTokenModel
            {
                AccessToken = CreateJWT(new JwtModel
                {
                    email = user.Email,
                    sub = user._id.ToString(),
                    role = user.Role
                }),
                // UserId = int.Parse(newCustomer.Properties["id"].ToString()),
                // Role = newCustomer.Properties["role"]?.ToString()
            }
        );
    }
    // [HttpGet]
    // [Route("refresh-token")]
    // public async Task<ActionResult> ValidateRefreshTokenAsync()
    // {
    //     string refreshToken = Request.Cookies["refreshToken"];
    //     Console.WriteLine(refreshToken);
    //     try
    //     {
    //         await driver.VerifyConnectivityAsync();
    //         await using var session = driver.AsyncSession();

    //         var query = neo4JQuery.QueryByOneElement(CUSTOMER, "refreshToken", "refreshToken", RETURN);
    //         var parameters = new Dictionary<string, object>
    //         {
    //             {"refreshToken",refreshToken}
    //         };
    //         var result = await neo4JQuery.ExecuteReadAsync(session, query, parameters);
    //         Console.WriteLine(result.Properties["ime"]);
    //         var dateNow = DateTime.UtcNow.ToLocalTime();
    //         var dateParse = DateTime.Parse(result.Properties["RTTimeExpire"].ToString()).ToLocalTime();
    //         if (result is not null && dateNow < dateParse)
    //         {
    //             var newRefreshToken = await GenerateAndSaveRefreshTokenAsync(new LoginModel(result.Properties["email"].ToString(), result.Properties["password"].ToString()));
    //             Response.Cookies.Append("refreshToken", newRefreshToken.ToString(), new CookieOptions
    //             {
    //                 HttpOnly = true,
    //                 Secure = true,
    //                 SameSite = SameSiteMode.Strict,
    //                 Expires = DateTimeOffset.UtcNow.AddDays(7)
    //             });
    //             return Ok(new ResponseTokenModel
    //             {
    //                 AccessToken = CreateJWT(new JwtModel
    //                 {
    //                     Email = result.Properties["email"]?.ToString(),
    //                     UserId = result.Properties["id"]?.ToString(),
    //                     Role = result.Properties["role"]?.ToString()
    //                 }),
    //                 UserId = int.Parse(result.Properties["id"]?.ToString()),
    //                 Role = result.Properties["role"]?.ToString(),
    //             });
    //         }
    //         return BadRequest("Refresh tokeh has expire!"); //ponovno logovanje
    //     }
    //     catch (Exception ex)
    //     {
    //         return NotFound(new { message = "False", error = ex });
    //     }
    // }
    [HttpGet]
    [Route("signout")]
    public void CustomerSignOut()
    {
        string newRefreshToken = "";
        Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });
    }
}