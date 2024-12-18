using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ERNI.Infrastructure;

internal class DatabaseFacadeProvider : IDatabaseFacadeProvider
{
    public IDatabaseFacadeShim ProvideFor(DbContext context)
    {
        var facade = context.Database;
        var shim = new DatabaseFacadeShim(facade);
        return shim;
    }

    private sealed class DatabaseFacadeShim(DatabaseFacade facade) : IDatabaseFacadeShim
    {
        public DatabaseProvider Provider
        {
            get
            {
                if (facade.IsNpgsql())
                {
                    return DatabaseProvider.PostgreSql;
                }

                return facade.IsInMemory()
                    ? DatabaseProvider.InMemory
                    : DatabaseProvider.Unknown;
            }
        }

        public async ValueTask MigrateAsync(CancellationToken token)
        {
            if (facade.IsRelational())
            {
                await facade.MigrateAsync(token);
            }
        }
    }
}
