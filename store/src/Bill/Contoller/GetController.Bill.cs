using Product.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ProductController
{
    [ApiController]
    [Route("bill")]
    public class GetBillController : ControllerBase
    {
        private readonly MongoClient client;

        public GetBillController()
        {
            client = new MongoClient(Environment.GetEnvironmentVariable("MONGODB_URI"));
        }
        [Route("get/all")]
        [HttpGet]
        public async Task<ActionResult> GetAllItemsAsync()
        {
            try
            {
                var billRef = client.GetDatabase("Store").GetCollection<BillModel>("Bill");
                var allItems = await billRef.Find<BillModel>(_ => true).ToListAsync();
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
                var billRef = client.GetDatabase("Store").GetCollection<BillModel>("Item");
                var filter = Builders<BillModel>.Filter.Eq(r => r._id, id);
                var getBill = await billRef.Find<BillModel>(filter).FirstOrDefaultAsync();
                return Ok(getBill);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = "False", error = ex });
            }
        }
        [Route("get/customer:{id}")]
        [HttpGet]
        public async Task<ActionResult> GetAllCustomersBillsAsync(string id)
        {
            try
            {
                var itemRef = client.GetDatabase("Store").GetCollection<BillModel>("Bill");
                var filter = Builders<BillModel>.Filter.Regex(x => x.CustomerId, new MongoDB.Bson.BsonRegularExpression(id, "i"));
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