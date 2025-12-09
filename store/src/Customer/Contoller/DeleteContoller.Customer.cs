using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Neo4j.Driver;

[ApiController]
[Route("customer")]
public class DeleteCustomerController : ControllerBase
{
    private readonly MongoClient client;
    private readonly IMongoDatabase db;
    private readonly IConfiguration configuration;
    public DeleteCustomerController(IConfiguration configuration)
    {
        this.configuration = configuration;
        client = new MongoClient(Environment.GetEnvironmentVariable("MONGODB_URI"));
        db = client.GetDatabase("Store");
    }

    // [Authorize(Roles = "Admin")]
    // [HttpGet]
    // [Route("auth")]
    // public IActionResult AutenticateOnlyEndpoint()
    // {
    //     return Ok("You are authenticated");
    // }

    //[Authorize(Roles = "Admin")]
    [HttpDelete]
    [Route("delete/{email}")]
    public async Task<ActionResult> DeleteCustomerAsync(string email)
    {
        try
        {
            var userRef = db.GetCollection<CustomerModel>("User");
            var filter = Builders<CustomerModel>.Filter.Eq(f => f.Email, email);
            var result = await userRef.FindOneAndDeleteAsync<CustomerModel>(filter);
            if (result is null)
            { return NotFound("Item not found"); }
            return Ok("Item successfuly deleted");
        }
        catch (Exception ex)
        {
            return NotFound(new { message = "False", error = ex });
        }
    }
}