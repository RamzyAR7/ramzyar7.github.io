using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_Service.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }
        public int? Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual List<Product> Products { get; set; }
        public virtual List<Category> Categories { get; set; }
        public virtual List<PriceList> PriceLists { get; set; }
    }
}
