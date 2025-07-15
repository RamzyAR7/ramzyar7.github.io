using JsTree.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Identity.Client;

namespace JsTree.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(c =>
            {
                c.HasKey(x => x.Id);
                c.Property(x => x.Name).IsRequired().HasMaxLength(100);
                c.Property(x => x.IsActive).IsRequired();
                c.HasMany(x => x.Courses)
                    .WithOne(x => x.Category)
                    .HasForeignKey(x => x.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Course>(c =>
            {
                c.HasKey(x => x.Id);
                c.Property(x => x.CategoryId).IsRequired();
            });

            base.OnModelCreating(modelBuilder);

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Programming", IsActive = true },
                new Category { Id = 2, Name = "Design", IsActive = true },
                new Category { Id = 3, Name = "Marketing", IsActive = true }
            );

            // Seed Courses
            modelBuilder.Entity<Course>().HasData(
                new Course { Id = 1, Name = "C# Basics", Instructor = "John Doe", Price = 49.99m, IsActive = true, CategoryId = 1 },
                new Course { Id = 2, Name = "Advanced JavaScript", Instructor = "Jane Smith", Price = 59.99m, IsActive = true, CategoryId = 1 },
                new Course { Id = 3, Name = "UI/UX Design", Instructor = "Alice Johnson", Price = 39.99m, IsActive = true, CategoryId = 2 },
                new Course { Id = 4, Name = "Digital Marketing 101", Instructor = "Bob Brown", Price = 29.99m, IsActive = true, CategoryId = 3 }
            );
        }
    }
}
