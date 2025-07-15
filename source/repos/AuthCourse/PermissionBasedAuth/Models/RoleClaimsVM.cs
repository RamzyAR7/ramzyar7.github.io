namespace PermissionBasedAuth.Models
{
    public class RoleClaimsVM
    {
        public string Id { get; set; }
        public string RoleName { get; set; }
        public List<ParentClaimVM> ParentClaims { get; set; }
    }
}
