using Product.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
//using Models;
namespace ProductController
{
    [ApiController]
    [Route("product")]
    public class GetProductController : ControllerBase
    {
        private readonly MongoClient client;

        public GetProductController()
        {

            client = new MongoClient(Environment.GetEnvironmentVariable("MONGODB_URI"));
        }
        [Route("get/all")]
        [HttpGet]
        public async Task<ActionResult> GetAllItemsAsync()
        {
            try
            {
                var itemRef = client.GetDatabase("Store").GetCollection<ProductModel>("Product");
                var allItems = await itemRef.Find(_ => true).ToListAsync();
                return Ok(allItems);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [Route("get/id:{id}")]
        [HttpGet]
        public async Task<ActionResult> GetItemByIdAsync(string id)
        {
            try
            {
                var itemRef = client.GetDatabase("Store").GetCollection<ProductModel>("Product");
                var filter = Builders<ProductModel>.Filter.Eq(r => r._id, id);
                var getItem = await itemRef.Find<ProductModel>(filter).FirstOrDefaultAsync();
                return Ok(getItem);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [Route("get/naziv:{naziv}")]
        [HttpGet]
        public async Task<ActionResult> GetItemNameAsync(string naziv)
        {
            try
            {
                var itemRef = client.GetDatabase("Store").GetCollection<ProductModel>("Product");
                var filter = Builders<ProductModel>.Filter.Regex(x => x.Name, new MongoDB.Bson.BsonRegularExpression(naziv, "i"));
                var resolts = await itemRef.Find(filter).ToListAsync();
                return Ok(resolts);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}