using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_Service.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public int? Code { get; set; }
        public string Name { get; set; }
        public int? CompanyId { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }

        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        public virtual ICollection<Price> Prices { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }

    }
}
