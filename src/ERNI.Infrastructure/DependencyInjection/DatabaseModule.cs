using Autofac;
using ERNI.Core;

namespace ERNI.Infrastructure;

public class DatabaseModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(ctx =>
        {
            var context = ctx.Resolve<ApplicationDbContext>();
            return context.Users;
        });

        builder.Register(ctx =>
        {
            var context = ctx.Resolve<ApplicationDbContext>();
            return context.Orders;
        });

        builder.RegisterType<DatabaseFacadeProvider>().As<IDatabaseFacadeProvider>();
        builder.RegisterType<MigrationsService>().As<IMigrationsService>();
        builder.RegisterGeneric(typeof(Repository<>))
            .As(typeof(IRepository<>))
            .InstancePerLifetimeScope();
    }
}
