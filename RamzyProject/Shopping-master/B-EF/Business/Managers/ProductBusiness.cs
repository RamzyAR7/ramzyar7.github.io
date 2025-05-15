using A_Service.IUnitOfWork;
using A_Service.Models;
using A_Service.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace B_EF.Business.Managers
{
    public interface IProductBusiness
    {
       Task<List<ProductViewModel>> GetAll();
        ProductViewModel GetProductById(int id);
        ProductViewModel GetProductByName(string Name);

        Product GetProducDbtById(int id);

    }
    public class ProductBusiness : IProductBusiness
    {
        private ApplicationDbContext _db;
        private IUnitOfWork _unitOfWork;
        public ProductBusiness(IUnitOfWork unitOfWork, ApplicationDbContext db)
        {
            this._unitOfWork = unitOfWork;
            this._db = db;

        }
        public async Task<List<ProductViewModel>> GetAll()
        {
            List<ProductViewModel> model;
            try
            {
                model =  _unitOfWork.ProductbBaseRepository.GetAllWithInclude(a => a.Prices, q => q.Category, c => c.Company).ToListAsync().Result.SelectMany(a => new List<ProductViewModel>() { new ProductViewModel()
                {
                           Id=a.Id,
                       Name = a.Name,
                                 Code = a.Code,
                       Description = a.Description,
                       CompanyId = a.CompanyId,
                       CompanyName = a.Company.Name,
                       CategoryName = a.Category.Name,
                       CreatedDate = a.CreatedDate,

                       CategoryId = a.CategoryId,
                       //PurchasingPrice =  a.Prices.FirstOrDefault().PurchasingPrice,
                       // SellingPrice = a.Prices.FirstOrDefault().SellingPrice,
                       // NosGomlaPrice = a.Prices.FirstOrDefault().NosGomlaPrice,
                        Prices = a.Prices.ToList()
                }

                }).ToList();

                return model;

            }
            catch (Exception ex)
            {

                throw;
            }
            return new List<ProductViewModel>() ;
        }

        public Product GetProducDbtById(int id)
        {

            return _db.Products.Where(a => a.Id == id).Include(c => c.ProductImages).Include(w=>w.Prices).FirstOrDefault();


        }

        public ProductViewModel GetProductById(int id)
        {
            ProductViewModel result = new ProductViewModel();
            var  model = _unitOfWork.ProductbBaseRepository.GetAllWithInclude(a => a.Prices, i=>i.ProductImages,q => q.Category, c => c.Company).FirstOrDefault(a => a.Id == id);
            if (model == null)
            {
                return result;
            }
            result.Id = model.Id;
            result.Name = model.Name;
            result.Code = model.Code;
            result.Description = model.Description;
            result.CompanyId = model.CompanyId;
            result.CategoryName = model.Category.Name;
            result.CompanyName = model.Company.Name;
            result.ProductImages = model.ProductImages?.ToList();

            result.CategoryId = model.CategoryId;
            result.PurchasingPrice = model.Prices.FirstOrDefault().PurchasingPrice;
            result.SellingPrice = model.Prices.FirstOrDefault().SellingPrice;
            result.NosGomlaPrice = model.Prices.FirstOrDefault().NosGomlaPrice;
            result.Prices = model.Prices.ToList();

            return result;
                 
        }

        public ProductViewModel GetProductByName(string Name)
        {
            ProductViewModel result = new ProductViewModel();
            var model = _unitOfWork.ProductbBaseRepository.GetAllWithInclude(a => a.Prices, i => i.ProductImages, q => q.Category, c => c.Company).FirstOrDefault(a => a.Name.Contains(Name));
            if (model == null)
            {
                return result;
            }
            result.Id = model.Id;
            result.Name = model.Name;
            result.Code = model.Code;
            result.Description = model.Description;
            result.CompanyId = model.CompanyId;
            result.CategoryName = model.Category.Name;
            result.CompanyName = model.Company.Name;
            result.ProductImages = model.ProductImages?.ToList();

            result.CategoryId = model.CategoryId;
            result.PurchasingPrice = model.Prices.FirstOrDefault().PurchasingPrice;
            result.SellingPrice = model.Prices.FirstOrDefault().SellingPrice;
            result.NosGomlaPrice = model.Prices.FirstOrDefault().NosGomlaPrice;
            result.Prices = model.Prices.ToList();

            return result;

        }
    }
}
