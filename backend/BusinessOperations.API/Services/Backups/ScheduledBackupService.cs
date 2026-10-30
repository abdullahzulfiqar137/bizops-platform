using BusinessOperations.API.Configurations;
using BusinessOperations.API.Interfaces.Services;
using Microsoft.Extensions.Options;

namespace BusinessOperations.API.Services.Backups;

public sealed class ScheduledBackupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly BackupSettings _settings;
    private readonly ILogger<ScheduledBackupService> _logger;

    public ScheduledBackupService(
        IServiceScopeFactory scopeFactory,
        IOptions<BackupSettings> settings,
        ILogger<ScheduledBackupService> logger)
    {
        _scopeFactory = scopeFactory;
        _settings = settings.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_settings.ScheduleEnabled)
        {
            return;
        }

        var interval = TimeSpan.FromHours(Math.Max(1, _settings.ScheduleIntervalHours));
        using var timer = new PeriodicTimer(interval);

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await using var scope = _scopeFactory.CreateAsyncScope();
                    var backupService = scope.ServiceProvider.GetRequiredService<IDatabaseBackupService>();
                    var result = await backupService.CreateAsync(stoppingToken);

                    if (!result.Success)
                    {
                        _logger.LogWarning("Scheduled database backup did not complete: {Message}", result.Message);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Scheduled database backup failed.");
                }
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Normal host shutdown.
        }
    }
}
