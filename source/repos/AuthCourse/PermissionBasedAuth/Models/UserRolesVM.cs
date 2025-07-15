namespace PermissionBasedAuth.Models
{
    public class UserRolesVM
    {
        public string UserId { get; set; }
        public string? UserName { get; set; }
        public List<CheckBoxVM> Roles { get; set; } = new List<CheckBoxVM>();
    }
}
