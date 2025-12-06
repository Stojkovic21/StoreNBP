using Item.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
namespace ItemController
{
    [ApiController]
    [Route("bill")]
    public class DeleteBillController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly MongoClient client;

        public DeleteBillController(IConfiguration configuration)
        {
            this.configuration = configuration;

            client = new MongoClient(Environment.GetEnvironmentVariable("MONGODB_URI"));
        }
        // [Authorize(Roles ="Admin")]
        [Route("delete/{id}")]
        [HttpDelete]
        public async Task<ActionResult> ObrisiBillAsync(string id)
        {
            try
            {
                var billRef = client.GetDatabase("Store").GetCollection<BillModel>("Bill");
                var filter = Builders<BillModel>.Filter.Eq(r => r._id, ObjectId.Parse(id));
                var result = await billRef.FindOneAndDeleteAsync(filter);
                //var result = await itemRef.DeleteOneAsync(filter);
                if (result is null)
                    return NotFound("Bill not found");
                return Ok("Bill successfuly deleted");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}