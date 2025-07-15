using PermissionAuth.Models;
using PermissionAuth.Repository;

namespace PermissionAuth.UnitOfWork
{
    public interface IUnitOfWork: IDisposable
    {
        IGenericRepository<User> User { get; }
        IGenericRepository<UserPermission> UserPermission { get; }
        IGenericRepository<Product> Product { get; }
        IGenericRepository<Order> Order { get; }


        Task SaveAsync();

        Task CommitAsync();

        Task RollbackAsync();

        Task BeginTransactionAsync();

        void Dispose();

    }
}
