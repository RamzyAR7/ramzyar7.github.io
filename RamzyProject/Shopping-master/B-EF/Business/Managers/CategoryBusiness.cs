using A_Service.IUnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B_EF.Business.Managers
{
    //public interface ICategoryBusiness
    //{
    //    bool CheckCategory(int id);
    //    bool CheckProduct(int id);
    //    bool CheckCompany(int id);



    //}
    //public class CategoryBusiness : ICategoryBusiness
    //{
    //    private ApplicationDbContext _db;
    //    private IUnitOfWork _unitOfWork;
    //    public CategoryBusiness(IUnitOfWork unitOfWork, ApplicationDbContext db)
    //    {
    //        this._unitOfWork = unitOfWork;
    //        this._db = db;

    //    }

    //    public bool CheckCategory(int id)
    //    {
    //      return  _db.Categories.Any(a => a.Id == id);
    //    }

    //    public bool CheckCompany(int id)
    //    {
    //        return _db.Categories.Where(a => a.CompanyId == id).Any();

    //    }

    //    public bool CheckProduct(int id)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
