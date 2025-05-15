using A_Service.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_Service.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public int? Code { get; set; }
        public string Name { get; set; }

        public string CompanyName { get; set; }
        public string CategoryName { get; set; }

        public int? CompanyId { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public decimal PurchasingPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal GomlaPrice { get; set; }
        public decimal NosGomlaPrice { get; set; }
        public DateTime CreatedDate { get; set; }


        public List<Price> Prices { get; set; }
        public List<IFormFile> photos { get; set; }

        public List<ProductImage> ProductImages { get; set; }

        public List<string> oldImage { get; set; }

        public string oldImageString { get; set; }



    }
}
