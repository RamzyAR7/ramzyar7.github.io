using System.ComponentModel.DataAnnotations;

namespace PermissionBasedAuth.Models
{
    public class RoleVM
    {
        [Required(ErrorMessage = "Role name is required")]
        [Display(Name = "Role Name")]
        public string Name { get; set; } = string.Empty;
    }
}
