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
    public async Task<ActionResult> EditCustomerAsync([FromBody] CustomerModel updateUser)
    {
        try
        {
            var userRef = db.GetCollection<CustomerModel>("User");
            var filter = Builders<CustomerModel>.Filter.Eq(f => f.Email, updateUser.Email);
            var user = await userRef.Find(filter).FirstOrDefaultAsync();
            if (Argon2.Verify(user.Password, updateUser.Password))
            {
                var result = await userRef.ReplaceOneAsync(filter, updateUser);
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