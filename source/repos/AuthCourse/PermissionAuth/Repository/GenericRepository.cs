using Microsoft.EntityFrameworkCore;
using PermissionAuth.Context;
using System.Linq.Expressions;

namespace PermissionAuth.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public T FindById(int id)
        {
            return _dbSet.Find(id);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }
        public IEnumerable<T> GetAll(Expression<Func<T, object>>? include)
        {
            if (include == null)
            {
                return GetAll();
            }
            else
            {
                _dbSet = (DbSet<T>)_dbSet.Include(include);
            }
            return _dbSet.ToList();
        }
        public IEnumerable<T> GetAll(params Expression<Func<T, object>>[] include)
        {
            if (include == null)
            {
                return GetAll();
            }
            else
            {
                foreach (var inc in include)
                {
                    if (_dbSet != null)
                    {
                        _dbSet = (DbSet<T>)_dbSet.Include(inc);
                    }
                }
            }
            return _dbSet.ToList();
        }
        // .Include("Category.Products"); like .include(c => c.Category).ThanInclude(p => p.Products)
        public IEnumerable<T> GetAll(params string[] includes)
        {
            if (includes == null || includes.Length == 0)
            {
                return GetAll();
            }
            else
            {
                foreach (var inc in includes)
                {
                    if (_dbSet != null)
                    {
                        _dbSet = (DbSet<T>)_dbSet.Include(inc);
                    }
                }
            }
            return _dbSet.ToList();
        }
        public IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] include)
        {
            if (include == null)
            {
                return _dbSet.Where(predicate).ToList();
            }
            else
            {
                foreach (var inc in include)
                {
                    if (_dbSet != null)
                    {
                        _dbSet = (DbSet<T>)_dbSet.Include(inc);
                    }
                }
            }
            return _dbSet.Where(predicate).ToList();
        }
    }
}
