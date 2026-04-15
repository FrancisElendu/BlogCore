using BlogCore.Application.Interfaces;
using BlogCore.Application.Interfaces.Services;
using BlogCore.Core.Entities;
using BlogCore.Infrastructure.Data;
using BlogCore.Infrastructure.Seeddata.Seeders;
using BlogCore.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MSSQLFlexCrud.DatatContext;
using MSSQLFlexCrud.Repositories;
using MSSQLFlexCrud.SqlDb;

namespace BlogCore.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<BlogDbContext>
            (options =>
                options.UseSqlServer
                (
                    config.GetConnectionString("DefaultConnection"),
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);

                    }
                )
            );

            // Register Identity with default settings (no custom options)
            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.User.RequireUniqueEmail = true; // Recommended for blogs
            })
            .AddEntityFrameworkStores<BlogDbContext>()
            .AddDefaultTokenProviders();

            // Register AppDbContext for backward compatibility (pointing to same database)
            services.AddScoped<AppDbContext>(provider => provider.GetRequiredService<BlogDbContext>());

            // Register custom services
            services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();

            // Register Auth Service
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            // Register UserManagementService
            services.AddScoped<IUserManagementService, UserManagementService>();

            // Register the generic repository from the NuGet package
            // It will work with AppDbContext (which will now be BlogDbContext)
            services.AddScoped(typeof(IRepository<>), typeof(SqlRepository<>));


        }
    }
}
