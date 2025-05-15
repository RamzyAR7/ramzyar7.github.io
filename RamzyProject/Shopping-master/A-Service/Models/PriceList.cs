using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_Service.Models
{
    public class PriceList
    {
        [Key]
        public int Id { get; set; }
        public int? Code { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }    
        public string Description { get; set; }
        public string FilePath { get; set; }

        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }
    }
}
