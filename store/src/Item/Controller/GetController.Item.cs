using Item.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
//using Models;
namespace ItemController
{
    [ApiController]
    [Route("item")]
    public class GetItemController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly MongoClient client;

        public GetItemController(IConfiguration configuration)
        {
            this.configuration = configuration;

            client = new MongoClient(Environment.GetEnvironmentVariable("MONGODB_URI"));
        }
        [Route("get/all")]
        [HttpGet]
        public async Task<ActionResult> GetAllItemsAsync()
        {
            try
            {
                var itemRef = client.GetDatabase("Store").GetCollection<ItemModel>("Item");
                var allItems = await itemRef.Find<ItemModel>(_ => true).ToListAsync();
                return Ok(allItems);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = "False", error = ex });
            }
        }
        [Route("get/id:{id}")]
        [HttpGet]
        public async Task<ActionResult> GetItemByIdAsync(string id)
        {
            try
            {
                var itemRef = client.GetDatabase("Store").GetCollection<ItemModel>("Item");
                var filter = Builders<ItemModel>.Filter.Eq(r => r._id, ObjectId.Parse(id));
                var getItem = await itemRef.Find<ItemModel>(filter).FirstOrDefaultAsync();
                return Ok(getItem);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = "False", error = ex });
            }
        }
        [Route("get/naziv:{naziv}")]
        [HttpGet]
        public async Task<ActionResult> GetItemNameAsync(string naziv)
        {
            try
            {
                var itemRef = client.GetDatabase("Store").GetCollection<ItemModel>("Item");
                var filter = Builders<ItemModel>.Filter.Regex(x => x.Name, new MongoDB.Bson.BsonRegularExpression(naziv, "i"));
                var resolts = await itemRef.Find(filter).ToListAsync();
                return Ok(resolts);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = "False", error = ex });
            }
        }
    }
}