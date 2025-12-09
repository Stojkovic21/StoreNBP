using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class BillModel
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string _id { get; set; }
    public int TotalPrice { get; set; }
    public List<OrderModel> Orders { get; set; }
    public DateTime Date { get; set; }
    public string CustomerId { get; set; }
    public string Note { get; set; }
}