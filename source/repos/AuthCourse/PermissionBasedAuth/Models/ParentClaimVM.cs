namespace PermissionBasedAuth.Models
{
    public class ParentClaimVM
    {
        public string ParentName { get; set; }
        public List<CheckBoxVM> ChildClaims { get; set; }
    }
}
