using Product.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Neo4j.Driver;
namespace ProductController
{
    [ApiController]
    [Route("product")]
    public class DeleteProductController : ControllerBase
    {
        private readonly MongoClient client;
        private readonly IDriver driver;
        private readonly Neo4jQuery neo4JQuery;
        public DeleteProductController()
        {
            var uri = Environment.GetEnvironmentVariable("URI");
            var user = Environment.GetEnvironmentVariable("Username");
            var password = Environment.GetEnvironmentVariable("Password");

            this.driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
            neo4JQuery = new();
            client = new MongoClient(Environment.GetEnvironmentVariable("MONGODB_URI"));
        }
        // [Authorize(Roles ="Admin")]
        [Route("delete/{id}")]
        [HttpDelete]
        public async Task<ActionResult> ObrisiItemAsync(string id)
        {
            try
            {
                var productRef = client.GetDatabase("Store").GetCollection<ProductModel>("Product");
                var filter = Builders<ProductModel>.Filter.Eq(r => r._id, id);
                var result = await productRef.FindOneAndDeleteAsync(filter);
                if (result is null)
                    return NotFound("Mongo product not found");

                await driver.VerifyConnectivityAsync();
                await using var session = driver.AsyncSession();

                var testQuety = neo4JQuery.QueryByOneElement("Product", "id", "id", "RETURN");
                var deleteQuery = @"
                MATCH (n:Product {id: $id})
                DETACH DELETE n";
                var parameters = new Dictionary<string, object>
                {
                    { "id", id }
                };

                var resultN = await neo4JQuery.ExecuteReadAsync(session, testQuety, parameters);
                if (resultN is null)
                    return NotFound("Neo4J product not found");

                var res = await neo4JQuery.ExecuteWriteAsync(session, deleteQuery, parameters);
                return Ok("Product successfuly deleted");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}