using Product.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Neo4j.Driver;
using NetTopologySuite.GeometriesGraph;

namespace ProductController
{
    [ApiController]
    [Route("bill")]
    public class GetBillController : ControllerBase
    {
        private readonly MongoClient client;
        private readonly IDriver driver;

        public GetBillController()
        {
            client = new MongoClient(Environment.GetEnvironmentVariable("MONGODB_URI"));
            var uri = Environment.GetEnvironmentVariable("URI");
            var user = Environment.GetEnvironmentVariable("Username");
            var password = Environment.GetEnvironmentVariable("Password");

            driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }
        [Route("get/all")]
        [HttpGet]
        public async Task<ActionResult> GetAllItemsAsync()
        {
            try
            {
                var billRef = client.GetDatabase("Store").GetCollection<BillModel>("Bill");
                var allItems = await billRef.Find(_ => true).ToListAsync();
                return Ok(allItems);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = "False", error = ex });
            }
        }
        [Route("get/{id}")]
        [HttpGet]
        public async Task<ActionResult> GetBillByIdAsyncEP(string id)
        {
            try
            {
                BillModel getBill = await GetBillByIDAsync(id);
                return Ok(getBill);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        private async Task<BillModel> GetBillByIDAsync(string id)
        {
            var billRef = client.GetDatabase("Store").GetCollection<BillModel>("Bill");
            var filter = Builders<BillModel>.Filter.Eq(r => r._id, id);
            var getBill = await billRef.Find(filter).FirstOrDefaultAsync();
            return getBill;
        }

        [Route("get/customer/{id}")]
        [HttpGet]
        public async Task<ActionResult> GetAllCustomersBillsAsync(string id)
        {
            try
            {
                await driver.VerifyConnectivityAsync();
                await using var session = driver.AsyncSession();

                string quary = @"MATCH p=(c:Customer{id: $customerid})-[:HAS_BILL]->(b:Bill) RETURN b.id as PRODUCTID";
                var parameters = new Dictionary<string, object> { { "customerid", id }, };
                var result = await session.ExecuteReadAsync(async (tx) =>
                {
                    var response = await tx.RunAsync(quary, parameters);
                    var nodes = new List<BillModel>();
                    var billid = new List<string>();
                    await response.ForEachAsync(reco =>
                    {
                        billid.Add(response.Current["PRODUCTID"].As<string>());

                    });
                    foreach (var item in billid)
                    {
                        BillModel bill = await GetBillByIDAsync(item);
                        nodes.Add(bill);
                    }
                    return nodes;
                });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}