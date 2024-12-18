using Autofac;

namespace ERNI.Server;

public static class ContainerBuilderExtensions
{
    public static ContainerBuilder RegisterHostedServices(this ContainerBuilder builder)
    {
        var hostedServiceTypes = typeof(ContainerBuilderExtensions)
            .Assembly
            .ExportedTypes
            .Where(x => x.IsAssignableTo<IHostedService>());

        foreach (var type in hostedServiceTypes)
        {
            builder.RegisterType(type).As<IHostedService>();
        }

        return builder;
    }
}
