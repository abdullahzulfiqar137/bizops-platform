using BusinessOperations.API.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace BusinessOperations.API.Tests.Configuration;

public sealed class ProductionConfigurationValidatorTests
{
    [Fact]
    public void Validate_AllowsDevelopmentConfiguration()
    {
        var configuration = BuildConfiguration(new Dictionary<string, string?>());

        ProductionConfigurationValidator.Validate(configuration, new TestEnvironment("Development"));
    }

    [Fact]
    public void Validate_RejectsMissingProductionSettings()
    {
        var configuration = BuildConfiguration(new Dictionary<string, string?>());

        var exception = Assert.Throws<InvalidOperationException>(() =>
            ProductionConfigurationValidator.Validate(configuration, new TestEnvironment("Production")));

        Assert.Contains("DefaultConnection", exception.Message);
        Assert.Contains("AllowedOrigins", exception.Message);
    }

    [Fact]
    public void Validate_AcceptsSecureProductionSettings()
    {
        var configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = "Server=database;Database=BizOps;User Id=app;Password=secret",
            ["JwtSettings:SecretKey"] = new string('x', 64),
            ["JwtSettings:Issuer"] = "https://api.example.com",
            ["JwtSettings:Audience"] = "https://app.example.com",
            ["JwtSettings:RequireHttpsMetadata"] = "true",
            ["AllowedOrigins:0"] = "https://app.example.com",
            ["DatabaseBackups:RetentionDays"] = "30",
            ["DatabaseBackups:ScheduleIntervalHours"] = "24"
        });

        ProductionConfigurationValidator.Validate(configuration, new TestEnvironment("Production"));
    }

    private static IConfiguration BuildConfiguration(Dictionary<string, string?> values) =>
        new ConfigurationBuilder().AddInMemoryCollection(values).Build();

    private sealed class TestEnvironment(string environmentName) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = environmentName;
        public string ApplicationName { get; set; } = "Tests";
        public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
