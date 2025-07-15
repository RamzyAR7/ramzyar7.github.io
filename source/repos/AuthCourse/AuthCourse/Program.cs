using Microsoft.AspNetCore.Builder;

namespace AuthCourse
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddAuthentication("MyCookies").AddCookie("MyCookies", Options =>
            {
                Options.Cookie.Name = "MyCookies";
                Options.ExpireTimeSpan = TimeSpan.FromDays(10);
                Options.LoginPath = "/Account/Login";
                Options.AccessDeniedPath = "/AccessDenied/Index";
            });

            builder.Services.AddAuthorization(option =>
            {
                option.AddPolicy("AdminOnly", policy => policy.RequireClaim("Admin", "True"));
                option.AddPolicy("ManagerHR", policy => policy.RequireClaim("Department", "HR").RequireClaim("Admin", "True"));
                option.AddPolicy("MustBelongToHR", policy => policy.RequireClaim("Department", "HR"));
            });

            builder.Services.AddSession(option =>
            {
                option.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout to 30 minutes
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "Area1",
                pattern: "api/{controller=Home}/{action=Index}/{id:int:range(1,1000)?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id:int:range(1,1000)?}");


            app.Run();
        }
    }
}
