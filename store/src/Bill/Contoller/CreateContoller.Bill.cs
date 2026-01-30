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
        private readonly MongoClient client;
        private readonly IDriver driver;
        private readonly Neo4jQuery neo4JQuery;
        private readonly IMongoDatabase db;
        private readonly ProductBillRelationship itembill;
        private readonly CustomerBillRelationship customerbill;
        public CreateBillController()
        {
            client = new MongoClient(Environment.GetEnvironmentVariable("MONGODB_URI"));
            db = client.GetDatabase("Store");
            var uri = Environment.GetEnvironmentVariable("URI");
            var user = Environment.GetEnvironmentVariable("Username");
            var password = Environment.GetEnvironmentVariable("Password");

            driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
            neo4JQuery = new();
            itembill = new ProductBillRelationship();
            customerbill = new CustomerBillRelationship();
        }
        //[Authoriza(Roles = "Admin")]
        [Route("create")]
        [HttpPost]
        public async Task<ActionResult> CreateBillAsync([FromBody] BillDTO products)
        {
            try
            {
                int totalPrice = 0;
                List<OrderModel> orders = new List<OrderModel>(); ;
                foreach (var item in products.Products)
                {
                    totalPrice += item.Price * item.Quantity;
                    orders.Add(new OrderModel { Product = new ProductModel { _id = item.Id, Name = item.Name, Price = item.Price, Description = "" }, Quantity = item.Quantity });
                }
                BillModel bill = new()
                {
                    Date = DateTime.UtcNow.ToLocalTime(),
                    CustomerId = Request.Cookies["customerID"],
                    Note = products.Note,
                    TotalPrice = totalPrice,
                    Orders = orders,
                };
                var billRef = db.GetCollection<BillModel>("Bill");

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

                // //connect bill with items neo4j

                foreach (var item in products.Products)
                {
                    await itembill.ConnectRelationshipProductBillAsync(new RelationshipModel { SourceId = item.Id, TargetId = bill._id, Count = item.Quantity });
                }
                await customerbill.ConncetRelationshiCustomerBillAsync(new RelationshipModel { SourceId = bill.CustomerId, TargetId = bill._id });

                return Ok("Bill added successfulyu");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}