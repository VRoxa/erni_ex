using Microsoft.Extensions.Logging;

namespace ERNI.Infrastructure;

internal sealed class MigrationsService : IMigrationsService
{
    private readonly ApplicationDbContext _context;
    private readonly IDatabaseFacadeProvider _facadeProvider;
    private readonly ILogger<MigrationsService> _logger;

    public MigrationsService(
        ApplicationDbContext context,
        IDatabaseFacadeProvider facadeProvider,
        ILogger<MigrationsService> logger)
    {
        _context = context;
        _facadeProvider = facadeProvider;
        _logger = logger;
    }

    public async Task MigrateAsync(CancellationToken token)
    {
        var facade = _facadeProvider.ProvideFor(_context);
        _logger.LogDebug("Applying missing migrations to database - {DbProvider}", facade.Provider);

        await facade.MigrateAsync(token);

        _logger.LogInformation("Migrations applied. Database is now up-to-date");
    }
}
