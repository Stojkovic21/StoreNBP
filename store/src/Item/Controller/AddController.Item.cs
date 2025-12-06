using Item.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Neo4j.Driver;
namespace ItemController
{
    [ApiController]
    [Route("item")]
    public class AddItemController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly MongoClient client;
        private readonly IDriver driver;
        private readonly Neo4jQuery neo4JQuery;
        private readonly IMongoDatabase db;

        public AddItemController(IConfiguration configuration)
        {
            this.configuration = configuration;

            client = new MongoClient(Environment.GetEnvironmentVariable("MONGODB_URI"));

            db = client.GetDatabase("Store");

            var uri = Environment.GetEnvironmentVariable("URI");
            var user = Environment.GetEnvironmentVariable("Username");
            var password = Environment.GetEnvironmentVariable("Password");

            driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
            neo4JQuery = new();
        }
        //[Authoriza(Roles = "Admin")]
        [Route("add")]
        [HttpPost]
        public async Task<ActionResult> AddItemAsync([FromBody] ItemModel item)
        {
            try
            {
                await driver.VerifyConnectivityAsync();
                await using var session = driver.AsyncSession();
                if (item.Name == "") return BadRequest("Unesite Naziv");
                var itemRef = db.GetCollection<ItemModel>("Item");

                await itemRef.InsertOneAsync(item);
                var query = @"
                CREATE(n:Item {id:$id})";
                var parameters = new Dictionary<string, object>
                {
                    {"id",item._id.ToString()},
                };
                var result = await neo4JQuery.ExecuteWriteAsync(session, query, parameters);
                return Ok("Item added successfulyu");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}