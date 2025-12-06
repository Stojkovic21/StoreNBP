using Item.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
namespace ItemController
{
    [ApiController]
    [Route("bill")]
    public class EditBillController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly MongoClient client;

        public EditBillController(IConfiguration configuration)
        {
            this.configuration = configuration;

            client = new MongoClient(Environment.GetEnvironmentVariable("MONGODB_URI"));
        }
        // [Authorize(Roles ="Admin")]
        [HttpPut]
        [Route("edit/{id}")]
        public async Task<ActionResult> EditItemAsync([FromBody] BillModel updatedBill, string id)
        {
            try
            {
                var billRef = client.GetDatabase("Store").GetCollection<BillModel>("Bill");
                var filter = Builders<BillModel>.Filter.Eq(r => r._id, ObjectId.Parse(id));
                var result = await billRef.ReplaceOneAsync(filter, updatedBill);
                if (result.ModifiedCount > 0)
                    return Ok("Bill is successfully updated");
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
    }// bill update za dodavanje ordera na bill
}