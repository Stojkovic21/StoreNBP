using Item.Models;
using MongoDB.Bson;
using Sprache;

public class OrderModel
{
    public ItemModel Item { get; set; }
    public double Count { get; set; }
    // public OrderModel(ItemModel item, int count)
    // {
    //     this.Item = item;
    //     this.Count = count;
    // }
    public double TotalPrice()
    {
        return Item.Price * Count;
    }
}