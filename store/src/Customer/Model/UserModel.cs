using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class UserModel
{
    public ObjectId _id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string RefreshToken { get; set; }
    [BsonElement("RTTimeExpire")]
    public DateTime RefreshTokenTimeExpire { get; set; }
    public void Print()
    {
        Console.WriteLine(Email + "\n" + Role + "\n" + Name + "\n" + LastName + "\n" + PhoneNumber + "\n" + RefreshToken + "\n" + RefreshTokenTimeExpire);
    }
}