using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ERNI.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        static string? GetConnectionString(IConfiguration configuration)
        {
            return Environment.GetEnvironmentVariable(Constants.ConnectionStringEnvironmentKey) ??
                configuration.GetConnectionString(Constants.ConnectionStringConfigurationKey);
        }

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var isInMemoryStr = configuration.GetSection(Constants.InMemoryFlagKey).Value ?? false.ToString();
            if (bool.TryParse(isInMemoryStr, out var isInMemory) && isInMemory)
            {
                options.UseInMemoryDatabase("ERNI");
                return;
            }

            var connectionString = GetConnectionString(configuration);
            options.UseNpgsql(
                connectionString,
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly));
        });

        return services;
    }
}
