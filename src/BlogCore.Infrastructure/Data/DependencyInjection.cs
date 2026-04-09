using BlogCore.Application.Interfaces;
using BlogCore.Infrastructure.Seeddata.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MSSQLFlexCrud.DatatContext;
using MSSQLFlexCrud.Repositories;
using MSSQLFlexCrud.SqlDb;

namespace BlogCore.Infrastructure.Data
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

            services.AddScoped<AppDbContext>(provider => provider.GetRequiredService<BlogDbContext>());

            services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();
            services.AddScoped(typeof(IRepository<>), typeof(SqlRepository<>));
        }
    }
}
