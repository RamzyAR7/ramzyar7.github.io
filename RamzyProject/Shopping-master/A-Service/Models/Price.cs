using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_Service.Models
{
    public class Price
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public decimal PurchasingPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal GomlaPrice { get; set; }
        public decimal NosGomlaPrice { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool? IsActive { get; set; }


        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
