using PermissionAuth.Dtos;
using System.Text.Json.Serialization;

namespace PermissionAuth.Models
{
    public class UserPermission
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public Permission Permission { get; set; }
        [JsonIgnore]
        public User user { get; set; }
    }
}
