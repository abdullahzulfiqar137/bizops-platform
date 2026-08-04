namespace BusinessOperations.API.Configurations;

public static class ProductionConfigurationValidator
{
    public static void Validate(IConfiguration configuration, IHostEnvironment environment)
    {
        if (!environment.IsProduction())
        {
            return;
        }

        var errors = new List<string>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            errors.Add("ConnectionStrings:DefaultConnection is required.");
        }
        else if (connectionString.Contains("DESKTOP-", StringComparison.OrdinalIgnoreCase) ||
                 connectionString.Contains("(localdb)", StringComparison.OrdinalIgnoreCase))
        {
            errors.Add("ConnectionStrings:DefaultConnection must not target a developer workstation or LocalDB.");
        }

        var jwt = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? new JwtSettings();
        if (!jwt.RequireHttpsMetadata)
        {
            errors.Add("JwtSettings:RequireHttpsMetadata must be true.");
        }

        ValidatePublicUrl(jwt.Issuer, "JwtSettings:Issuer", errors);
        ValidatePublicUrl(jwt.Audience, "JwtSettings:Audience", errors);

        var origins = configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];
        if (origins.Length == 0)
        {
            errors.Add("At least one AllowedOrigins entry is required.");
        }

        foreach (var origin in origins)
        {
            ValidatePublicUrl(origin, "AllowedOrigins", errors);
        }

        var backup = configuration.GetSection(BackupSettings.SectionName).Get<BackupSettings>() ?? new BackupSettings();
        if (backup.RetentionDays < 7)
        {
            errors.Add("DatabaseBackups:RetentionDays must be at least 7.");
        }

        if (backup.ScheduleEnabled && backup.ScheduleIntervalHours < 1)
        {
            errors.Add("DatabaseBackups:ScheduleIntervalHours must be at least 1.");
        }

        if (errors.Count > 0)
        {
            throw new InvalidOperationException(
                "Production configuration is invalid:" + Environment.NewLine +
                string.Join(Environment.NewLine, errors.Select(error => $"- {error}")));
        }
    }

    private static void ValidatePublicUrl(string? value, string key, ICollection<string> errors)
    {
        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
        {
            errors.Add($"{key} must be an absolute HTTPS URL.");
            return;
        }

        if (uri.Host.Contains("yourdomain", StringComparison.OrdinalIgnoreCase) ||
            uri.IsLoopback)
        {
            errors.Add($"{key} must not contain a placeholder or loopback host.");
        }
    }
}
