using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Neo4j.Driver;
using Product.Models;
using ProductController;
[ApiController]
[Route("recommendation")]
public class Recommendation : ControllerBase
{
    private readonly IDriver driver;
    private readonly GetProductController getProduct;
    private readonly MongoClient client;
    public Recommendation()
    {
        client = new MongoClient(Environment.GetEnvironmentVariable("MONGODB_URI"));

        var uri = Environment.GetEnvironmentVariable("URI");
        var user = Environment.GetEnvironmentVariable("Username");
        var password = Environment.GetEnvironmentVariable("Password");

        driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        getProduct = new();
    }
    private async Task<List<string>> AlsoBuyFromNeo4J(string productId)
    {
        try
        {
            await driver.VerifyConnectivityAsync();
            await using var session = driver.AsyncSession();

            var quary = @"MATCH (:Product {id: $productId})<-[:PRODUCT_ON_BILL]-(:Bill)<-[:HAS_BILL]-(c:Customer)
                        MATCH (c)-[:HAS_BILL]->(:Bill)-[:PRODUCT_ON_BILL]->(rec:Product)
                        WHERE rec.id <> $productId
                        RETURN rec.id as ProductId, count(*) as score
                        ORDER BY score DESC
                        LIMIT 5";
            var parameters = new Dictionary<string, object> { { "productId", productId } };
            var result = await session.ExecuteReadAsync(async tx =>
            {
                var response = await tx.RunAsync(quary, parameters);
                var nodes = new List<string>();
                await response.ForEachAsync(record =>
                {
                    nodes.Add(response.Current["ProductId"].As<string>());
                });
                return nodes;
            });

            return result;
        }
        catch (Exception)
        {
            throw;
        }
    }
    [Route("alsobuy/{productId}")]
    [HttpGet]
    public async Task<ActionResult> AlsoBuy(string productId)
    {
        try
        {
            var alsoBuyProducts = await AlsoBuyFromNeo4J(productId);
            List<ProductModel> products = new();
            foreach (var id in alsoBuyProducts)
            {
                var itemRef = client.GetDatabase("Store").GetCollection<ProductModel>("Product");
                var filter = Builders<ProductModel>.Filter.Eq(r => r._id, id);
                var getItem = await itemRef.Find<ProductModel>(filter).FirstOrDefaultAsync();
                products.Add(getItem);
            }
            return Ok(products);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}