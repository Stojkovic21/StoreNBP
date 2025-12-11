using Product.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
//using Models;
namespace ProductController
{
    [ApiController]
    [Route("product")]
    public class EditProductController : ControllerBase
    {
        private readonly MongoClient client;

        public EditProductController()
        {
            client = new MongoClient(Environment.GetEnvironmentVariable("MONGODB_URI"));
        }
        // [Authorize(Roles ="Admin")]
        [HttpPut]
        [Route("edit/{id}")]
        public async Task<ActionResult> EditItemAsync([FromBody] ProductModel updatedProduct, string id)
        {
            try
            {
                var itemRef = client.GetDatabase("Store").GetCollection<ProductModel>("Product");
                var filter = Builders<ProductModel>.Filter.Eq(r => r._id, id);
                var result = await itemRef.ReplaceOneAsync(filter, updatedProduct);
                if (result.ModifiedCount > 0)
                    return Ok("Product is successfully updated");
                return BadRequest("Something went wrong");

                //Update
                // var update = Builders<ProductModel>.Update.Set(r => r.Name, updatedItem.Name);
                // var options = new FindOneAndUpdateOptions<ProductModel> { ReturnDocument = ReturnDocument.After };
                // var result = itemRef.FindOneAndUpdateAsync(filter, update, options);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}