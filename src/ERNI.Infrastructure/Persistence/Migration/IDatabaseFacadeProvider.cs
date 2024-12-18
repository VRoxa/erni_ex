using Microsoft.EntityFrameworkCore;

namespace ERNI.Infrastructure;

internal interface IDatabaseFacadeProvider
{
    IDatabaseFacadeShim ProvideFor(DbContext context);
}
