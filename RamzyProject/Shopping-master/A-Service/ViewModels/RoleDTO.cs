using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_Service.ViewModels
{
    public class RoleDTO
    {
        public string Id { get; set; }

        [Display(Name = "RoleName")]
        [Required]
        public string Name { get; set; }

        public bool Selected { get; set; }
    }
}
