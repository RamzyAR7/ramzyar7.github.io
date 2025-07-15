using System.Linq.Expressions;

namespace PermissionAuth.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        T FindById(int id);
        void Add(T entity);
        void Remove(T entity);
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAll(Expression<Func<T, object>>? include);
        IEnumerable<T> GetAll(params Expression<Func<T, object>>[] includes);
        IEnumerable<T> GetAll(params string[] includes);
        IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

        void Save();
    }
}
