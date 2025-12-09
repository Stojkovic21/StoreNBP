using Product.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Neo4j.Driver;
namespace ProductController
{
    [ApiController]
    [Route("bill")]
    public class CreateBillController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly MongoClient client;
        private readonly IDriver driver;
        private readonly Neo4jQuery neo4JQuery;

        public CreateBillController(IConfiguration configuration)
        {
            this.configuration = configuration;
            client = new MongoClient(Environment.GetEnvironmentVariable("MONGODB_URI"));

            var uri = Environment.GetEnvironmentVariable("URI");
            var user = Environment.GetEnvironmentVariable("Username");
            var password = Environment.GetEnvironmentVariable("Password");

            driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
            neo4JQuery = new();
        }
        //[Authoriza(Roles = "Admin")]
        [Route("create")]
        [HttpPost]
        public async Task<ActionResult> CreateBillAsync([FromBody] BillModel bill)
        {
            try
            {
                //if (bill.Name == "") return BadRequest("Unesite Naziv");
                var billRef = client.GetDatabase("Store").GetCollection<BillModel>("Bill");

                await billRef.InsertOneAsync(bill);

                await driver.VerifyConnectivityAsync();
                await using var session = driver.AsyncSession();

                var query = @"
                CREATE(n:Bill {id:$id})";
                var parameters = new Dictionary<string, object>
                {
                    {"id",bill._id.ToString()},
                };
                var result = await neo4JQuery.ExecuteWriteAsync(session, query, parameters);

                return Ok("Bill added successfulyu");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}