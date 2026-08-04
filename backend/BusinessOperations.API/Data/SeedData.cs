using BusinessOperations.API.Constants;
using BusinessOperations.API.Entities.Expenses;
using BusinessOperations.API.Entities.Identity;
using BusinessOperations.API.Entities.Products;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperations.API.Data;

public static class SeedData
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IConfiguration configuration,
        ILogger logger)
    {
        // 1. Seed Roles
        foreach (var roleName in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new ApplicationRole
                {
                    Name = roleName,
                    Description = $"{roleName} Role with appropriate permissions"
                });
            }
        }

        // 2. Seed Admin User
        var adminEmail = configuration["InitialAdmin:Email"];
        var adminUsername = configuration["InitialAdmin:Username"];
        var adminPassword = configuration["InitialAdmin:Password"];
        var hasPinnedTenant = Guid.TryParse(configuration["Tenancy:PinnedTenantId"], out var pinnedTenantId);
        if (!string.IsNullOrWhiteSpace(adminEmail) &&
            !string.IsNullOrWhiteSpace(adminUsername) &&
            !string.IsNullOrWhiteSpace(adminPassword) &&
            hasPinnedTenant)
        {
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminUsername,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Admin",
                    EmailConfirmed = true,
                    IsActive = true,
                    TenantId = pinnedTenantId
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, Roles.Admin);
                    logger.LogWarning(
                        "Initial administrator created. Remove InitialAdmin configuration after first successful sign-in.");
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Initial administrator could not be created: {string.Join("; ", result.Errors.Select(error => error.Description))}");
                }
            }
        }
        else
        {
            logger.LogInformation(
                "InitialAdmin or Tenancy:PinnedTenantId configuration is not present; automatic administrator creation was skipped.");
        }

        // 3. Seed Default Product Measurement Units
        if (!await context.ProductUnits.AnyAsync())
        {
            var defaultUnits = new List<ProductUnit>
            {
                new ProductUnit { Id = Guid.NewGuid(), Name = "Piece", Abbreviation = "pcs" },
                new ProductUnit { Id = Guid.NewGuid(), Name = "Kilogram", Abbreviation = "kg" },
                new ProductUnit { Id = Guid.NewGuid(), Name = "Liter", Abbreviation = "L" },
                new ProductUnit { Id = Guid.NewGuid(), Name = "Box", Abbreviation = "box" },
                new ProductUnit { Id = Guid.NewGuid(), Name = "Pack", Abbreviation = "pk" }
            };

            await context.ProductUnits.AddRangeAsync(defaultUnits);
            await context.SaveChangesAsync();
        }

        // Business categories are tenant-owned data and are created by users.

        // 4. Seed Subscription Plans & Active Subscription
        if (!await context.SubscriptionPlans.AnyAsync())
        {
            var starterPlan = new BusinessOperations.API.Entities.Subscriptions.SubscriptionPlan
            {
                Id = Guid.NewGuid(),
                Name = "Starter Plan",
                Code = "STARTER",
                MonthlyPrice = 29,
                AnnualPrice = 290,
                MaxUsers = 3,
                MaxSalesPerMonth = 250,
                EnableAiAssistant = false,
                EnableDoubleEntryAccounting = false,
                EnablePdfGeneration = true,
                Description = "Essential point-of-sale and inventory tracking for small retail stores.",
                CreatedBy = "System",
                CreatedAtUtc = DateTime.UtcNow
            };

            var proPlan = new BusinessOperations.API.Entities.Subscriptions.SubscriptionPlan
            {
                Id = Guid.NewGuid(),
                Name = "Professional Plan",
                Code = "PRO",
                MonthlyPrice = 79,
                AnnualPrice = 790,
                MaxUsers = 15,
                MaxSalesPerMonth = 2500,
                EnableAiAssistant = true,
                EnableDoubleEntryAccounting = true,
                EnablePdfGeneration = true,
                Description = "Complete ERP operations with PDF invoices, SignalR alerts, and AI insights.",
                CreatedBy = "System",
                CreatedAtUtc = DateTime.UtcNow
            };

            var enterprisePlan = new BusinessOperations.API.Entities.Subscriptions.SubscriptionPlan
            {
                Id = Guid.NewGuid(),
                Name = "Enterprise VIP Plan",
                Code = "ENTERPRISE",
                MonthlyPrice = 199,
                AnnualPrice = 1990,
                MaxUsers = 100,
                MaxSalesPerMonth = 100000,
                EnableAiAssistant = true,
                EnableDoubleEntryAccounting = true,
                EnablePdfGeneration = true,
                Description = "Unlimited high-volume business operations with dedicated AI assistant & automated backups.",
                CreatedBy = "System",
                CreatedAtUtc = DateTime.UtcNow
            };

            await context.SubscriptionPlans.AddRangeAsync(starterPlan, proPlan, enterprisePlan);
            await context.SaveChangesAsync();

        }
    }
}
