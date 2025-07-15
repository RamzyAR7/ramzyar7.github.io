using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PermissionBasedAuth.Data;
using PermissionBasedAuth.Entities;
using PermissionBasedAuth.Filter;
using PermissionBasedAuth.Seeding;
using System.Net;

namespace PermissionBasedAuth
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ApplicationDbContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection"));
            });
            builder.Services.AddScoped<AuthorizationHelper>();
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(option =>
            {
                // Password settings
                option.Password.RequiredLength = 6;
                option.Password.RequireDigit = true;
                option.Password.RequireLowercase = true;
                option.Password.RequireUppercase = true;
                option.Password.RequireNonAlphanumeric = false;
                option.User.RequireUniqueEmail = true;

            }).AddEntityFrameworkStores<ApplicationDbContext>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    // Seed roles
                    await IdentitySeeding.SeedingRolesAsync(roleManager);

                    // Seed users
                    await IdentitySeeding.SeedingUsersAsync(userManager, roleManager);

                    Console.WriteLine("Database seeding completed successfully.");
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
