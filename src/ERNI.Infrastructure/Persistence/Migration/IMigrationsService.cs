namespace ERNI.Infrastructure;

public interface IMigrationsService
{
    Task MigrateAsync(CancellationToken token);
}
