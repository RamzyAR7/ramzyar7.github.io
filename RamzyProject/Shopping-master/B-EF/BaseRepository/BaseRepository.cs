using A_Service.IBaseRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace B_EF.BaseRepository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly ApplicationDbContext _dbContext;
        private IQueryable<T> DbSet;

        public BaseRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            DbSet = _dbContext.Set<T>().AsQueryable();


        }

        public async Task<ICollection<T>> GetAll()=> await _dbContext.Set<T>().ToListAsync();
        
        public async void Add(T entity)
        {
           await _dbContext.Set<T>().AddAsync(entity);
        }

        public async Task<T> GetById(int id)
        {
           return await _dbContext.Set<T>().FindAsync(id);
        }

        public void Delete(int id)
        {        
           _dbContext.Set<T>().Remove(_dbContext.Set<T>().Find(id));
        }

        public IQueryable<T> GetAllWithInclude(params Expression<Func<T, object>>[] includes)
        {

            if (includes != null)
            {
                return includes.Aggregate(DbSet
                                    , (current, includeProperty) => current.Include(includeProperty)).AsQueryable();
            }
            return DbSet.AsNoTracking().AsQueryable();
        }

        public bool Exists(Expression<Func<T, bool>> whereCondition)
        {
            return _dbContext.Set<T>().Any(whereCondition);
        }
    }
}
