using Microsoft.EntityFrameworkCore;
using PermissionAuth.Configrations;
using PermissionAuth.Models;

namespace PermissionAuth.Context
{
    public class ApplicationDbContext:DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options ) :base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfigrations());
            modelBuilder.ApplyConfiguration(new OrderConfigrations());
            base.OnModelCreating(modelBuilder);

        }
    }
}
