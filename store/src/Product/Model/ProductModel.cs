using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Product.Models
{
    public class ProductModel
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Weight_g { get; set; }
        public List<String> With { get; set; }
        public string Description { get; set; }
    }
}