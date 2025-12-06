using Item.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
//using Models;
namespace ItemController
{
    [ApiController]
    [Route("item")]
    public class EditItemController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly MongoClient client;

        public EditItemController(IConfiguration configuration)
        {
            this.configuration = configuration;

            client = new MongoClient(Environment.GetEnvironmentVariable("MONGODB_URI"));
        }
        // [Authorize(Roles ="Admin")]
        [HttpPut]
        [Route("edit/{id}")]
        public async Task<ActionResult> EditItemAsync([FromBody] ItemModel updatedItem, string id)
        {
            try
            {
                var itemRef = client.GetDatabase("Store").GetCollection<ItemModel>("Item");
                var filter = Builders<ItemModel>.Filter.Eq(r => r._id, ObjectId.Parse(id));
                var result = await itemRef.ReplaceOneAsync(filter, updatedItem);
                if (result.ModifiedCount > 0)
                    return Ok("Item is successfully updated");
                return BadRequest("Something went wrong");

                //Update
                // var update = Builders<ItemModel>.Update.Set(r => r.Name, updatedItem.Name);
                // var options = new FindOneAndUpdateOptions<ItemModel> { ReturnDocument = ReturnDocument.After };
                // var result = itemRef.FindOneAndUpdateAsync(filter, update, options);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}