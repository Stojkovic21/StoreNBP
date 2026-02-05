using Product.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Neo4j.Driver;
namespace ProductController
{
    [ApiController]
    [Route("product")]
    public class AddProductController : ControllerBase
    {
        private readonly MongoClient client;
        private readonly IDriver driver;
        private readonly Neo4jQuery neo4JQuery;
        private readonly IMongoDatabase db;

        public AddProductController()
        {
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
        public async Task<ActionResult> AddItemAsync([FromBody] ProductModel product)
        {
            try
            {
                await driver.VerifyConnectivityAsync();
                await using var session = driver.AsyncSession();
                if (product.Name == "") return BadRequest("Unesite naziv");
                var productRef = db.GetCollection<ProductModel>("Product");

                await productRef.InsertOneAsync(product);
                var query = @"
                CREATE(n:Product {id:$id})";
                var parameters = new Dictionary<string, object>
                {
                    {"id",product._id}
                };
                var result = await neo4JQuery.ExecuteWriteAsync(session, query, parameters);
                return Ok("Product added successfulyu");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromForm] UploadImageDTO image)
        {
            if (image.Image == null || image.Image.Length == 0)
                return BadRequest("Fajl nije poslat");

            var uploadFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "..",
            "test-react",
            "src",
            "Images"
            );
            //var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            //ovo cemo pretpostaviti za sad da nece imati isto ime
            var path = Path.Combine(uploadFolder, image.Title);

            using var stream = new FileStream(path, FileMode.Create);
            await image.Image.CopyToAsync(stream);

            return Ok("Image is successfully saved");
        }
    }
}