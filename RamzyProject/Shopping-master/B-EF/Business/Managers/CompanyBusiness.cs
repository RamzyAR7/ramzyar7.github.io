using A_Service.IUnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B_EF.Business.Managers
{
    public interface ICompanyBusiness
    {
        bool CheckCompany(int id);

    }
    public class CompanyBusiness : ICompanyBusiness
    {
        private ApplicationDbContext _db;
        private IUnitOfWork _unitOfWork;
        public CompanyBusiness(IUnitOfWork unitOfWork, ApplicationDbContext db)
        {
            this._unitOfWork = unitOfWork;
            this._db = db;

        }

        public bool CheckCompany(int id)
        {
            return _db.Categories.Any(a => a.Id == id);
        }
    }
}
