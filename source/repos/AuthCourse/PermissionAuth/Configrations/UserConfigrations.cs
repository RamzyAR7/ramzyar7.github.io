using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PermissionAuth.Models;

namespace PermissionAuth.Configrations
{
    public class UserConfigrations : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.Id)
                 .IsRequired()
                 .UseIdentityColumn();

            builder.Property(u => u.Name)
                .IsRequired();

            builder.Property(u => u.Password)
                .IsRequired();

            builder.HasMany(u => u.Permissions)
                .WithOne(p => p.user)
                .HasForeignKey(p => p.UserId);
        }
    }
}
