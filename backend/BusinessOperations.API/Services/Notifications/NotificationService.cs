using BusinessOperations.API.Data;
using BusinessOperations.API.DTOs.Notifications;
using BusinessOperations.API.Entities.Notifications;
using BusinessOperations.API.Enums;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperations.API.Services.Notifications;

public sealed class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public NotificationService(ApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<NotificationCenterResponse>> GetAsync(CancellationToken cancellationToken = default)
    {
        var userId = RequireUser();
        await SynchronizeAsync(userId, cancellationToken);

        var notifications = await _db.UserNotifications.AsNoTracking()
            .Where(x => x.UserId == userId && x.IsActive)
            .OrderBy(x => x.IsRead).ThenByDescending(x => x.UpdatedAtUtc)
            .Take(50)
            .Select(x => new NotificationResponse
            {
                Id = x.Id, Type = x.Type, Title = x.Title, Message = x.Message,
                Route = x.Route, IsRead = x.IsRead, CreatedAtUtc = x.CreatedAtUtc
            }).ToListAsync(cancellationToken);

        return ApiResponse<NotificationCenterResponse>.SuccessResponse(new NotificationCenterResponse
        {
            UnreadCount = notifications.Count(x => !x.IsRead),
            Notifications = notifications
        });
    }

    public async Task<ApiResponse<bool>> MarkReadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var userId = RequireUser();
        var item = await _db.UserNotifications.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);
        if (item is null) return ApiResponse<bool>.FailureResponse("Notification not found.");
        item.IsRead = true;
        item.ReadAtUtc = DateTime.UtcNow;
        item.UpdatedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return ApiResponse<bool>.SuccessResponse(true, "Notification marked as read.");
    }

    public async Task<ApiResponse<bool>> MarkAllReadAsync(CancellationToken cancellationToken = default)
    {
        var userId = RequireUser();
        var now = DateTime.UtcNow;
        await _db.UserNotifications
            .Where(x => x.UserId == userId && x.IsActive && !x.IsRead)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.IsRead, true)
                .SetProperty(x => x.ReadAtUtc, now)
                .SetProperty(x => x.UpdatedAtUtc, now), cancellationToken);
        return ApiResponse<bool>.SuccessResponse(true, "All notifications marked as read.");
    }

    private async Task SynchronizeAsync(Guid userId, CancellationToken token)
    {
        var now = DateTime.UtcNow;
        var cutoff = now.AddDays(-30);
        var alerts = new List<Alert>();

        alerts.AddRange(await _db.Products.AsNoTracking()
            .Where(x => x.IsActive && x.StockQuantity <= x.MinimumStockLevel)
            .Select(x => new Alert($"low-stock:{x.Id}", "warning", "Low stock", $"{x.Name} has {x.StockQuantity} remaining (minimum {x.MinimumStockLevel}).", "/inventory"))
            .ToListAsync(token));

        alerts.AddRange(await _db.Sales.AsNoTracking()
            .Where(x => x.DueAmount > 0 && x.SaleDateUtc <= cutoff && x.Status != SaleStatus.Cancelled && x.Status != SaleStatus.Returned)
            .Select(x => new Alert($"sale-overdue:{x.Id}", "danger", "Customer payment overdue", $"{x.InvoiceNumber} has an outstanding balance older than 30 days.", "/sales"))
            .ToListAsync(token));

        alerts.AddRange(await _db.Purchases.AsNoTracking()
            .Where(x => x.DueAmount > 0 && x.PurchaseDateUtc <= cutoff && x.Status != PurchaseStatus.Cancelled && x.Status != PurchaseStatus.Returned)
            .Select(x => new Alert($"purchase-overdue:{x.Id}", "warning", "Supplier payment due", $"{x.PurchaseOrderNumber} has an outstanding balance older than 30 days.", "/purchases"))
            .ToListAsync(token));

        var current = await _db.UserNotifications.Where(x => x.UserId == userId).ToListAsync(token);
        var activeKeys = alerts.Select(x => x.Key).ToHashSet(StringComparer.Ordinal);
        foreach (var existing in current.Where(x => x.IsActive && !activeKeys.Contains(x.SourceKey)))
        {
            existing.IsActive = false;
            existing.UpdatedAtUtc = now;
        }

        foreach (var alert in alerts)
        {
            var existing = current.FirstOrDefault(x => x.SourceKey == alert.Key);
            if (existing is null)
            {
                _db.UserNotifications.Add(new UserNotification
                {
                    UserId = userId, SourceKey = alert.Key, Type = alert.Type, Title = alert.Title,
                    Message = alert.Message, Route = alert.Route, CreatedAtUtc = now, UpdatedAtUtc = now
                });
            }
            else
            {
                existing.Type = alert.Type;
                existing.Title = alert.Title;
                existing.Message = alert.Message;
                existing.Route = alert.Route;
                existing.IsActive = true;
                existing.UpdatedAtUtc = now;
            }
        }
        await _db.SaveChangesAsync(token);
    }

    private Guid RequireUser() => _currentUser.UserId ?? throw new UnauthorizedAccessException("Authentication is required.");
    private sealed record Alert(string Key, string Type, string Title, string Message, string Route);
}
