using BusinessOperations.API.DTOs.Notifications;
using BusinessOperations.API.Models;

namespace BusinessOperations.API.Interfaces.Services;

public interface INotificationService
{
    Task<ApiResponse<NotificationCenterResponse>> GetAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> MarkReadAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> MarkAllReadAsync(CancellationToken cancellationToken = default);
}
