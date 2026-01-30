using Product.Models;
using MongoDB.Bson;
using Sprache;

public class OrderModel
{
    public ProductModel Product { get; set; }
    public double Quantity { get; set; }
    // public OrderModel(ItemModel item, int count)
    // {
    //     this.Item = item;
    //     this.Count = count;
    // }
    public double TotalPrice()
    {
        return Product.Price * Quantity;
    }
}