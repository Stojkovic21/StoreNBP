using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Neo4j.Driver;

[ApiController]
[Route("customer")]
public class EditCustomerController : ControllerBase
{
    private readonly MongoClient client;
    private readonly IMongoDatabase db;
    public EditCustomerController()
    {
        client = new MongoClient(Environment.GetEnvironmentVariable("MONGODB_URI"));
        db = client.GetDatabase("Store");
    }
    //[Authorize(Roles = "Admin,User")]
    [HttpPut]
    [Route("update")]
    public async Task<ActionResult> EditCustomerAsync([FromBody] CustomerModel updateCustomer)
    {
        try
        {
            var customerRef = db.GetCollection<CustomerModel>("Customer");
            var filter = Builders<CustomerModel>.Filter.Eq(f => f.Email, updateCustomer.Email);
            var customer = await customerRef.Find(filter).FirstOrDefaultAsync();
            // Console.WriteLine(customer);
            if (Argon2.Verify(customer.Password, updateCustomer.Password))
            {
                var result = await customerRef.ReplaceOneAsync(filter, updateCustomer);
                if (result.ModifiedCount > 0)
                    return Ok("Item is successfully updated");
                return BadRequest("Something went wrong");
            }
            else { return BadRequest("Enter correct password"); }

        }
        catch (System.Exception ex)
        {
            return NotFound(new { message = "False", error = ex });
        }
    }
    //Change email or password
}