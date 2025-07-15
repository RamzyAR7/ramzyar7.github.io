using Microsoft.EntityFrameworkCore.Storage;
using PermissionAuth.Context;
using PermissionAuth.Models;
using PermissionAuth.Repository;

namespace PermissionAuth.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext DbContext;
        private IDbContextTransaction? _transaction;

        private IGenericRepository<User> _userRepository;
        private IGenericRepository<UserPermission> _userPermissionRepository;
        private IGenericRepository<Product> _productRepository;
        private IGenericRepository<Order> _orderRepository;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        // more memory efficient way to use repositories than creating them every time in ctor even we don't use them (private fields + public Getter) 
        // search Lazy initialization vs Constructor Assignment for more details
        public IGenericRepository<User> User => _userRepository ??= new GenericRepository<User>(DbContext);
        // vs 
        // public IGenericRepository<User> User => _userRepository ?? new GenericRepository<User>(DbContext); without assignment to private field return directly new instance every time 
        // and in the sec call to User property it will create new instance of GenericRepository<User> which is not what we want

        public IGenericRepository<UserPermission> UserPermission => _userPermissionRepository ??= new GenericRepository<UserPermission>(DbContext);
        public IGenericRepository<Product> Product => _productRepository ??= new GenericRepository<Product>(DbContext);
        public IGenericRepository<Order> Order => _orderRepository ??= new GenericRepository<Order>(DbContext);


        public async Task SaveAsync()
        {
            await DbContext.SaveChangesAsync();
        }

        public async Task CommitAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                DbContext.Dispose();

            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                DbContext.Dispose(); 
            }
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction == null)
            {
                _transaction = await DbContext.Database.BeginTransactionAsync();
            }
            else
            {
                throw new InvalidOperationException("A transaction is already in progress.");
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            DbContext.Dispose();
        }
    }
}
