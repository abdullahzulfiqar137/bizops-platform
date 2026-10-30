using BusinessOperations.API.Configurations;
using BusinessOperations.API.DTOs.Backups;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace BusinessOperations.API.Services.Backups;

public sealed class DatabaseBackupService : IDatabaseBackupService
{
    private readonly BackupSettings _settings;
    private readonly string _connectionString;
    private readonly string _backupDirectory;
    private readonly ILogger<DatabaseBackupService> _logger;

    public DatabaseBackupService(
        IConfiguration configuration,
        IOptions<BackupSettings> settings,
        IWebHostEnvironment environment,
        ILogger<DatabaseBackupService> logger)
    {
        _settings = settings.Value;
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");
        _backupDirectory = Path.GetFullPath(Path.IsPathRooted(_settings.Directory)
            ? _settings.Directory
            : Path.Combine(environment.ContentRootPath, _settings.Directory));
        _logger = logger;
    }

    public Task<ApiResponse<IReadOnlyList<DatabaseBackupResponse>>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!System.IO.Directory.Exists(_backupDirectory))
            {
                return Task.FromResult(ApiResponse<IReadOnlyList<DatabaseBackupResponse>>.SuccessResponse(
                    Array.Empty<DatabaseBackupResponse>()));
            }

            IReadOnlyList<DatabaseBackupResponse> backups = System.IO.Directory
                .EnumerateFiles(_backupDirectory, "*.bak", SearchOption.TopDirectoryOnly)
                .Select(path => new FileInfo(path))
                .OrderByDescending(file => file.CreationTimeUtc)
                .Select(file => new DatabaseBackupResponse
                {
                    FileName = file.Name,
                    SizeBytes = file.Length,
                    CreatedAtUtc = file.CreationTimeUtc
                })
                .ToArray();

            return Task.FromResult(ApiResponse<IReadOnlyList<DatabaseBackupResponse>>.SuccessResponse(backups));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to list database backups.");
            return Task.FromResult(ApiResponse<IReadOnlyList<DatabaseBackupResponse>>.FailureResponse(
                "Database backups could not be listed."));
        }
    }

    public async Task<ApiResponse<DatabaseBackupResponse>> CreateAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            System.IO.Directory.CreateDirectory(_backupDirectory);

            var connectionBuilder = new SqlConnectionStringBuilder(_connectionString);
            if (string.IsNullOrWhiteSpace(connectionBuilder.InitialCatalog))
            {
                return ApiResponse<DatabaseBackupResponse>.FailureResponse(
                    "The database name is missing from the connection string.");
            }

            var fileName = $"{SanitizeFileName(connectionBuilder.InitialCatalog)}_{DateTime.UtcNow:yyyyMMdd_HHmmssfff}.bak";
            var backupPath = Path.Combine(_backupDirectory, fileName);
            var databaseName = QuoteIdentifier(connectionBuilder.InitialCatalog);

            await using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                await using var command = connection.CreateCommand();
                command.CommandText = $"BACKUP DATABASE {databaseName} TO DISK = @backupPath WITH INIT, CHECKSUM;";
                command.Parameters.AddWithValue("@backupPath", backupPath);
                command.CommandTimeout = 0;
                await command.ExecuteNonQueryAsync(cancellationToken);
            }

            if (!string.IsNullOrWhiteSpace(_settings.OffsiteDirectory))
            {
                var offsiteDirectory = Path.GetFullPath(Path.IsPathRooted(_settings.OffsiteDirectory)
                    ? _settings.OffsiteDirectory
                    : Path.Combine(AppContext.BaseDirectory, _settings.OffsiteDirectory));
                System.IO.Directory.CreateDirectory(offsiteDirectory);
                File.Copy(backupPath, Path.Combine(offsiteDirectory, fileName), overwrite: true);
            }

            ApplyRetentionPolicy();

            var info = new FileInfo(backupPath);
            return ApiResponse<DatabaseBackupResponse>.SuccessResponse(new DatabaseBackupResponse
            {
                FileName = info.Name,
                SizeBytes = info.Length,
                CreatedAtUtc = info.CreationTimeUtc
            }, "Database backup created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database backup creation failed.");
            return ApiResponse<DatabaseBackupResponse>.FailureResponse(
                "Database backup creation failed. Verify SQL Server backup permissions and backup storage access.");
        }
    }

    public async Task<ApiResponse<bool>> RestoreAsync(
        RestoreDatabaseRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!_settings.EnableRestore)
        {
            return ApiResponse<bool>.FailureResponse("Database restore is disabled by configuration.");
        }

        if (!string.Equals(request.Confirmation, "RESTORE", StringComparison.Ordinal))
        {
            return ApiResponse<bool>.FailureResponse("Explicit RESTORE confirmation is required.");
        }

        var backupPath = ResolveBackupPath(request.FileName);
        if (backupPath is null || !File.Exists(backupPath))
        {
            return ApiResponse<bool>.FailureResponse("The requested backup file was not found.");
        }

        var builder = new SqlConnectionStringBuilder(_connectionString);
        var databaseName = builder.InitialCatalog;
        if (string.IsNullOrWhiteSpace(databaseName))
        {
            return ApiResponse<bool>.FailureResponse("The database name is missing from the connection string.");
        }

        builder.InitialCatalog = "master";
        var quotedDatabaseName = QuoteIdentifier(databaseName);

        try
        {
            await using var connection = new SqlConnection(builder.ConnectionString);
            await connection.OpenAsync(cancellationToken);

            await using (var singleUser = connection.CreateCommand())
            {
                singleUser.CommandText = $"ALTER DATABASE {quotedDatabaseName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";
                await singleUser.ExecuteNonQueryAsync(cancellationToken);
            }

            try
            {
                await using var restore = connection.CreateCommand();
                restore.CommandText = $"RESTORE DATABASE {quotedDatabaseName} FROM DISK = @backupPath WITH REPLACE, RECOVERY;";
                restore.Parameters.AddWithValue("@backupPath", backupPath);
                restore.CommandTimeout = 0;
                await restore.ExecuteNonQueryAsync(cancellationToken);
            }
            finally
            {
                await using var multiUser = connection.CreateCommand();
                multiUser.CommandText = $"ALTER DATABASE {quotedDatabaseName} SET MULTI_USER;";
                await multiUser.ExecuteNonQueryAsync(cancellationToken);
            }

            return ApiResponse<bool>.SuccessResponse(true, "Database restored successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database restore failed for backup {BackupFile}.", request.FileName);
            return ApiResponse<bool>.FailureResponse(
                "Database restore failed. Verify SQL Server restore permissions and ensure the backup is compatible.");
        }
    }

    public string? ResolveBackupPath(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName) ||
            !string.Equals(Path.GetFileName(fileName), fileName, StringComparison.Ordinal) ||
            !fileName.EndsWith(".bak", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var fullPath = Path.GetFullPath(Path.Combine(_backupDirectory, fileName));
        var root = _backupDirectory.EndsWith(Path.DirectorySeparatorChar)
            ? _backupDirectory
            : _backupDirectory + Path.DirectorySeparatorChar;

        return fullPath.StartsWith(root, OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal) ? fullPath : null;
    }

    private void ApplyRetentionPolicy()
    {
        if (_settings.RetentionDays <= 0 || !System.IO.Directory.Exists(_backupDirectory))
        {
            return;
        }

        var cutoff = DateTime.UtcNow.AddDays(-_settings.RetentionDays);
        foreach (var file in new DirectoryInfo(_backupDirectory).EnumerateFiles("*.bak"))
        {
            if (file.CreationTimeUtc < cutoff)
            {
                file.Delete();
            }
        }
    }

    private static string QuoteIdentifier(string value) => $"[{value.Replace("]", "]]", StringComparison.Ordinal)}]";

    private static string SanitizeFileName(string value)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var safe = new string(value.Select(character => invalid.Contains(character) ? '_' : character).ToArray());
        return string.IsNullOrWhiteSpace(safe) ? "database" : safe;
    }
}
