using Item.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using Neo4j.Driver;
//using Models;
namespace ItemController
{
    [ApiController]
    [Route("item")]
    public class DeleteItemController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly MongoClient client;

        public DeleteItemController(IConfiguration configuration)
        {
            this.configuration = configuration;

            client = new MongoClient(Environment.GetEnvironmentVariable("MONGODB_URI"));
        }
        // [Authorize(Roles ="Admin")]
        [Route("delete/{id}")]
        [HttpDelete]
        public async Task<ActionResult> ObrisiItemAsync(string id)
        {
            try
            {
                var itemRef = client.GetDatabase("Store").GetCollection<ItemModel>("Item");
                var filter = Builders<ItemModel>.Filter.Eq(r => r._id, ObjectId.Parse(id));
                var result = await itemRef.FindOneAndDeleteAsync(filter);
                //var result = await itemRef.DeleteOneAsync(filter);
                if (result is null)
                    return NotFound("Item not found");
                return Ok("Item successfuly deleted");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}