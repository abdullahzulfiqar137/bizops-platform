using BusinessOperations.API.Data;
using BusinessOperations.API.Configurations;
using BusinessOperations.API.Entities.Identity;
using BusinessOperations.API.Extensions;
using BusinessOperations.API.Middleware;
using Microsoft.AspNetCore.Identity;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

ProductionConfigurationValidator.Validate(builder.Configuration, builder.Environment);

// Configure Serilog Logger
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddDatabaseServices(builder.Configuration);
builder.Services.AddIdentityServices();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSecurityAndCors(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

// Configure HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Business Operations API v1");
    });
}

app.UseRouting();
app.UseStaticFiles();

app.UseCors("Frontend");
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireRateLimiting("FixedPolicy");
app.MapHub<BusinessOperations.API.Hubs.NotificationHub>("/hubs/notifications");

// Apply EF Core Migrations and Seed Initial SuperAdmin Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

        await DbInitializer.InitializeAsync(
            context,
            userManager,
            roleManager,
            builder.Configuration,
            logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while seeding database initial data.");
        throw;
    }
}

try
{
    Log.Information("Starting Business Operations API...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
