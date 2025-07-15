namespace PermissionBasedAuth.Models
{
    public class UsersVM
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public ICollection<string> Roles { get; set; } = new List<string>();
    }
}
