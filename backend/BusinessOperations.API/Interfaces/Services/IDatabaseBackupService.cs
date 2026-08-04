using BusinessOperations.API.DTOs.Backups;
using BusinessOperations.API.Models;

namespace BusinessOperations.API.Interfaces.Services;

public interface IDatabaseBackupService
{
    Task<ApiResponse<IReadOnlyList<DatabaseBackupResponse>>> ListAsync(
        CancellationToken cancellationToken = default);
    Task<ApiResponse<DatabaseBackupResponse>> CreateAsync(
        CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> RestoreAsync(
        RestoreDatabaseRequest request,
        CancellationToken cancellationToken = default);
    string? ResolveBackupPath(string fileName);
}
