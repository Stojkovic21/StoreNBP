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
        public string Description { get; set; }
        public string Image { get; set; }
    }
    public class UploadImageDTO
    {
        public IFormFile Image { get; set; }
        public string Title { get; set; }
    }
}