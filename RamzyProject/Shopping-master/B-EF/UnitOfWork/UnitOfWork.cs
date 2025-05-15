using A_Service.IBaseRepository;
using A_Service.IUnitOfWork;
using A_Service.Models;
using B_EF.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B_EF.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public IBaseRepository<Product> ProductbBaseRepository { get; set; }
        public IBaseRepository<Company> CompanyBaseRepository { get; set; }
        public IBaseRepository<Category> CategoryBaseRepository { get; set; }
        public IBaseRepository<PriceList> PriceListBaseRepository { get; set; }
        public IBaseRepository<Price> PricesBaseRepository { get; set; }


        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            ProductbBaseRepository = new BaseRepository<Product>(_dbContext);
            CompanyBaseRepository = new BaseRepository<Company>(_dbContext);
            CategoryBaseRepository = new BaseRepository<Category>(_dbContext);
            PriceListBaseRepository = new BaseRepository<PriceList>(_dbContext);
            PricesBaseRepository = new BaseRepository<Price>(_dbContext);
        }

        
       
        public async Task<int> saveChanges() =>await _dbContext.SaveChangesAsync();
        
        public void Dispose() => _dbContext.Dispose();


    }
}
