namespace ERNI.Infrastructure;

internal enum DatabaseProvider
{
    PostgreSql,
    InMemory,
    Unknown
}

internal interface IDatabaseFacadeShim
{
    DatabaseProvider Provider { get; }
    
    ValueTask MigrateAsync(CancellationToken token);
}
