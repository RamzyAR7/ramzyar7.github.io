using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UserIdentity.Models
{
    public class RoleVM
    {
        [Required(ErrorMessage = "Role Name is required.")]
        [DisplayName("Role Name")]
        public string RoleName { get; set; }
    }
}
