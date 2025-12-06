using MongoDB.Bson;
namespace Item.Models
{
    public class ItemModel
    {
        public ObjectId _id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Weight_g { get; set; }
        public List<String> With { get; set; }
    }
}