using BusinessOperations.API.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperations.API.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IConfiguration configuration,
        ILogger logger)
    {
        try
        {
            try
            {
                logger.LogInformation("Applying database migrations if required...");
                await context.Database.MigrateAsync();
                logger.LogInformation("Database migration completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Database migration failed.");
                throw;
            }

            await SeedData.SeedAsync(
                context,
                userManager,
                roleManager,
                configuration,
                logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }
}
