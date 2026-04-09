using BlogCore.Application.Interfaces;

namespace BlogCore.API
{
    public static class DependencyInjection
    {
        public static async Task SeedDatabaseAsync(this WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
            {
                app.Logger.LogInformation("Skipping database seeding - not in development environment");
                return;
            }

            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
                    await seeder.SeedAsync();
                    app.Logger.LogInformation("Database seeding completed successfully.");
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "An error occurred while seeding the database.");
                    throw; // Re-throw to prevent app startup with bad data
                }
            }
        }
    }
}
