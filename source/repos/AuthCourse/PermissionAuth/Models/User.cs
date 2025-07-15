using PermissionAuth.Dtos;

namespace PermissionAuth.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
        public List<UserPermission> Permissions { get; set; } = new List<UserPermission>();
        public List<Order> orders { get; set; }
    }
}
