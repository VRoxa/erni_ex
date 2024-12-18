using Autofac;

namespace ERNI.Server;

public static class ApplicationBuilderExtensions
{
    public static void UseMiddlewares(this IApplicationBuilder app)
    {
        var middlewareTypes = typeof(ApplicationBuilderExtensions)
            .Assembly
            .ExportedTypes
            .Where(x => x.IsAssignableTo<IMiddleware>());

        foreach (var type in middlewareTypes)
        {
            app.UseMiddleware(type);
        }
    }
}
