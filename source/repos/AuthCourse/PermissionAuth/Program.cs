 
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PermissionAuth.ActionFilters;
using PermissionAuth.Context;
using PermissionAuth.Dtos;
using PermissionAuth.MiddleWares;
using PermissionAuth.Service;
using System.Text;

namespace PermissionAuth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddJsonFile("Config.json");
            // Add services to the container.

            builder.Services.AddControllers();
            var jwt = builder.Configuration.GetSection("jwt").Get<Jwt>();

            builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration.GetConnectionString("DefaultConnection"));
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<LogInfoFilter>();
            });

            
            builder.Services.AddScoped<CheckPassword>();
            builder.Services.AddScoped<GenrateToken>();
            builder.Services.AddScoped<Authorization>();
            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton(jwt);
            builder.Services.AddEndpointsApiExplorer();
            // if we work with IMiddleware we need to add this line
            builder.Services.AddTransient<GlobalExceptionHandlingMiddleware>();
            builder.Services.AddTransient<ProfileMiddleware>();
            builder.Services.AddScoped<RateLimitingMiddleware>();
            builder.Services.AddTransient<RateLImitingV2Middleware>();

            builder.Services.AddAuthentication()
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, Options =>
                {
                    Options.SaveToken = true;
                    Options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,


                        ValidIssuer = jwt.Issuer,
                        ValidAudience = jwt.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwt.SigningKey)),

                    };
                });

            var info = new OpenApiInfo()
            {
                Title = "Test",
                Version = "vi",
                Description = "Test"
            };

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", info);

                var xmlFile = "DocumentationSetting.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);  
                options.IncludeXmlComments(xmlPath);

                options.DocInclusionPredicate((name, api) => true);
                options.OrderActionsBy((apiDesc) =>
                {
                    var groupName = apiDesc.GroupName ?? "Default";
                    return groupName switch
                    {
                        "Account" => "0",
                        "Product" => "1",
                        "User" => "2",
                        _ => "3"
                    };
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseMiddleware<RateLimitingMiddleware>();
            app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
            app.UseMiddleware<ProfileMiddleware>();
            // cors middleware
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
//app.Use(async (context, next) =>
//{
//    try
//    {
//        await next(context);
//    }
//    catch (Exception ex)
//    {
//        context.Response.StatusCode = 500;
//        await context.Response.WriteAsJsonAsync(new { message = ex.Message });
//        await next(context);
//    }
//});
// Register the global exception handling middleware