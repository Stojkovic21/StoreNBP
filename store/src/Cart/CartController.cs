using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Operation.Buffer;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis; //dotnet add package NRedisStack

[ApiController]
[Route("cart")]
public class CartController : ControllerBase
{
    private readonly ConnectionMultiplexer muxer;
    private readonly IDatabase db;
    public CartController()
    {
        muxer = ConnectionMultiplexer.Connect(
            new ConfigurationOptions
            {
                EndPoints = { { "redis-17361.c293.eu-central-1-1.ec2.cloud.redislabs.com", 17361 } },
                User = "default",
                Password = "L95TqI1CiBLhUKKNLt6cIvr7NtP6OF6d",
                AbortOnConnectFail = false,
            }
        );
        db = muxer.GetDatabase();
    }
    [HttpPost]
    [Route("new")]
    public ActionResult AddItemInCart([FromBody] CartModel cartProduct)
    {
        try
        {
            JsonCommands json = db.JSON();
            var key = "Product " + cartProduct.Id;
            string customer = Request.Cookies["customerID"];
            var product = new Dictionary<string, object>
            {
                {"Id",cartProduct.Id},
                {"Name", cartProduct.Name},
                {"Price", cartProduct.Price},
                {"Quantity", cartProduct.Quantity}
            };
            if (!db.KeyExists(customer))
            {
                json.Set(customer, "$", "{}");
            }
            json.Set(customer, "$." + cartProduct.Id, product);
            return Ok("Uspesno dodato u bazu");
        }
        catch (System.Exception ex)
        {
            return BadRequest("Doslo je do greske prilikom povezivanja na bazu\n" + ex);
        }
    }
    [HttpPatch]
    [Route("inc/{productId}/{inc}")]
    public ActionResult IncrementCounter(string productId, string inc)
    {
        string customer = Request.Cookies["customerID"];
        try
        {
            JsonCommands json = db.JSON();
            json.NumIncrby(customer, $"$.{productId}.Quantity", double.Parse(inc));

            return Ok("Increment successfully");
        }
        catch (System.Exception x)
        {
            return BadRequest(x);
        }
    }
    [HttpDelete]
    [Route("remove/{id}")]
    public ActionResult RemoveItemFromCart(string id)
    {
        string customer = Request.Cookies["customerID"];
        try
        {
            JsonCommands json = db.JSON();
            json.Del(customer, $"$.{id}");
            return Ok("Product is removed successfully");
        }
        catch (System.Exception)
        {
            return BadRequest("Something went wrong");
        }
    }
    [HttpGet]
    [Route("get")]
    public ActionResult GetAllItemsFromCurt()
    {
        string customer = Request.Cookies["customerID"];
        try
        {
            var db = muxer.GetDatabase();
            var json = db.JSON();

            if (db.KeyExists(customer))
            {

                var products = json.Get<Dictionary<string, CartModel>>(customer);

                return Ok(products);
            }
            return BadRequest("Key's not found");
        }
        catch (System.Exception x)
        {
            return BadRequest(x);
        }
    }
    [HttpGet]
    [Route("get/{id}")]
    public ActionResult GetItemById(string id)
    {
        string customer = Request.Cookies["customerID"];
        try
        {
            var json = db.JSON();
            if (db.KeyExists(customer))
            {
                var result = json.Get<CartModel>(customer, $"$.{id}");
                return Ok(result);
            }
            return BadRequest("Item does not exist");
        }
        catch (System.Exception)
        {
            throw;
        }
    }
}

