using ERNI.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ERNI.Integration.Tests;

public class ApplicationFactory<T> : WebApplicationFactory<Program>
    where T : Program
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Force replace default database with an in memory version.
            var descriptor = services.SingleOrDefault(x =>
            {
                return x.ServiceType == typeof(DbContextOptions<ApplicationDbContext>);
            });

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("Testing");
            });

            var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();
        });
    }
}
