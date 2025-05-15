using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_Service.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public int? Code { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }
        public string Description { get; set; }

        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }

        public virtual List<Product> Products { get; set; }

    }
}
