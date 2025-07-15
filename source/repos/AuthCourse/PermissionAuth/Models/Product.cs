using System.Text.Json.Serialization;

namespace PermissionAuth.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        [JsonIgnore]
        public List<Order> orders { get; set; }

    }
}
