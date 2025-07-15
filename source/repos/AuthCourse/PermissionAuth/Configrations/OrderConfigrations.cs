using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PermissionAuth.Models;

namespace PermissionAuth.Configrations
{
    public class OrderConfigrations : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasOne(o => o.Product)
                .WithMany(p => p.orders)
                .HasForeignKey(o => o.ProductId);

            builder.HasOne(o => o.User)
                .WithMany(u => u.orders)
                .HasForeignKey(o => o.UserId);
        }
    }
}
