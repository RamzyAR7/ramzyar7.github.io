using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace A_Service.IBaseRepository
{
    public interface IBaseRepository<T> where T: class
    {
        Task<ICollection<T>> GetAll();

        void Add(T entity);

        bool Exists(Expression<Func<T, bool>> whereCondition);
        Task<T> GetById(int id);

        void Delete(int id);
        IQueryable<T> GetAllWithInclude(params Expression<Func<T, object>>[] includes);
    }
}
