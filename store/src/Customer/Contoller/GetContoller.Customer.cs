using System.Net;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Neo4j.Driver;

[ApiController]
[Route("customer")]
public class GetCustomerController : ControllerBase
{
    private readonly IConfiguration configuration;
    private readonly MongoClient client;
    private readonly IMongoDatabase db;
    public GetCustomerController(IConfiguration configuration)
    {
        this.configuration = configuration;
        client = new MongoClient(Environment.GetEnvironmentVariable("MONGODB_URI"));
        db = client.GetDatabase("Store");
    }
    [Authorize(Roles = "Admin")]
    [HttpGet]
    [Route("get/all")]
    public async Task<ActionResult> GetAllCustomerAsync()
    {
        try
        {
            var userRef = db.GetCollection<CustomerModel>("User");
            var allUsers = await userRef.Find<CustomerModel>(_ => true).ToListAsync();
            return Ok(allUsers);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = "False", error = ex });
        }
    }
    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult> GetCustomerAsync(string id)
    {
        try
        {
            var userRef = db.GetCollection<CustomerModel>("User");
            var filter = Builders<CustomerModel>.Filter.Eq(f => f._id, id);
            var user = await userRef.Find(filter).FirstOrDefaultAsync();
            return Ok(user);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = "False", error = ex });
        }
    }
    // [HttpGet]
    // [Route("me")]
    // public async Task<ActionResult> GetMe()
    // {
    //     AuthCustomerController authCustomerController = new(configuration);
    //     try
    //     {
    //         string refreshToken = Request.Cookies["refreshToken"].ToString();
    //         Console.WriteLine(refreshToken);
    //         await driver.VerifyConnectivityAsync();
    //         await using var session = driver.AsyncSession();

    //         var query = neo4JQuary.QueryByOneElement("Customer", "refreshToken", "refreshToken", RETURN);
    //         // var parameters = new Dictionary<string, object>
    //         // {
    //         //     {"refreshToken",refreshToken}
    //         // };

    //         // var result = await neo4JQuary.ExecuteReadAsync(session,query, parameters);

    //         // if (result != null)
    //         // {
    //         //     return Ok(new ResponseTokenModel
    //         //     {
    //         //         AccessToken = authCustomerController.CreateJWT(new JwtModel
    //         //         {
    //         //             Email = result.Properties["email"]?.ToString(),
    //         //             UserId = result.Properties["id"]?.ToString(),
    //         //             Role = result.Properties["role"]?.ToString()
    //         //         }),
    //         //         UserId =int.Parse(result.Properties["id"]?.ToString()),
    //         //         Role = result.Properties["role"]?.ToString(),
    //         //     });
    //         // }
    //         //else
    //         return NotFound(new { message = "Customer is not found" });
    //     }
    //     catch (Exception ex)
    //     {
    //         return NotFound(new { message = "False", error = ex });
    //     }
    // }
}
