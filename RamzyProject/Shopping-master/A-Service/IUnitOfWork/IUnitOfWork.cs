using A_Service.IBaseRepository;
using A_Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_Service.IUnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<Product> ProductbBaseRepository { get; set; }
        IBaseRepository<Company> CompanyBaseRepository { get; set; }
        IBaseRepository<Category> CategoryBaseRepository { get; set; }
        IBaseRepository<PriceList> PriceListBaseRepository { get; set; }
        IBaseRepository<Price> PricesBaseRepository { get; set; }

        Task<int> saveChanges();
    }
}
