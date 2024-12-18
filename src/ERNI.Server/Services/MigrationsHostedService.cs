
using ERNI.Infrastructure;

namespace ERNI.Server.Services;

public class MigrationsHostedService : IHostedService
{
    private readonly IMigrationsService _migrationService;

    public MigrationsHostedService(IMigrationsService migrationService)
    {
        _migrationService = migrationService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _migrationService.MigrateAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
