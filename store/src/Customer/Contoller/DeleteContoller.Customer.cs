using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Neo4j.Driver;
using ProductController;

[ApiController]
[Route("customer")]
public class DeleteCustomerController : ControllerBase
{
    private readonly MongoClient client;
    private readonly IMongoDatabase db;
    private readonly IDriver driver;
    private readonly Neo4jQuery neo4JQuery;
    private readonly DeleteBillController deleteBillController;
    public DeleteCustomerController()
    {
        var uri = Environment.GetEnvironmentVariable("URI");
        var user = Environment.GetEnvironmentVariable("Username");
        var password = Environment.GetEnvironmentVariable("Password");

        driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        client = new MongoClient(Environment.GetEnvironmentVariable("MONGODB_URI"));
        db = client.GetDatabase("Store");
        neo4JQuery = new();
        deleteBillController = new();
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
            var customerRef = db.GetCollection<CustomerModel>("Customer");
            var filter = Builders<CustomerModel>.Filter.Eq(f => f.Email, email);
            var result = await customerRef.FindOneAndDeleteAsync<CustomerModel>(filter);
            if (result is null)
            { return NotFound("Customer not found"); }
            return Ok("Customer successfuly deleted");
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
    [HttpDelete]
    [Route("delete")]
    public async Task<ActionResult> DelteMyAccount()
    {
        string customerId = Request.Cookies["customerID"];
        try
        {
            var customerRef = db.GetCollection<CustomerModel>("Customer");
            var filter = Builders<CustomerModel>.Filter.Eq(f => f._id, customerId);
            var result = await customerRef.FindOneAndDeleteAsync<CustomerModel>(filter);

            await driver.VerifyConnectivityAsync();
            await using var session = driver.AsyncSession();
            var testQuety = @"MATCH (n:Customer{id:$customerId}) 
            return n";
            string deleteQuery = @"MATCH (n:Customer{id:$customerId}) 
            DETACH DELETE n";
            var parameters = new Dictionary<string, object>
                {
                    { "customerId", customerId }
                };
            var resultN = await neo4JQuery.ExecuteReadAsync(session, testQuety, parameters);
            if (resultN is null)
                return NotFound("Customer not found");
            string billQuary = @"MATCH (c:Customer{id: $customerId})-[:HAS_BILL]->(b:Bill) 
            DETACH DELETE b;";
            var deleteBill = await neo4JQuery.ExecuteReadAsync(session, billQuary, parameters);
            var res = await neo4JQuery.ExecuteWriteAsync(session, deleteQuery, parameters);
            return Ok("Successfully deleted");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}