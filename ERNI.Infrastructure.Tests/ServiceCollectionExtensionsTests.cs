using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ERNI.Infrastructure.Tests;

public class ServiceCollectionExtensionsTests
{
    private const string _envConnectionString = "Host=localhost;Port=5432;Database=environment;Username=erni;Password=erni";
    private const string _configConnectionString = "Host=localhost;Port=5432;Database=configuration;Username=erni;Password=erni";

    private static IConfiguration FakeInMemoryConfiguration
    {
        get
        {
            var configuration = new Dictionary<string, string>
            {
                { Constants.InMemoryFlagKey, true.ToString() }
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(configuration)
                .Build();
        }
    }

    private static IConfiguration FakeExternalDatabaseConfiguration(string? inMemoryFlagValue = null)
    {
        var configuration = new Dictionary<string, string?>
        {
            { Constants.InMemoryFlagKey, inMemoryFlagValue },
            { $"ConnectionStrings:{Constants.ConnectionStringConfigurationKey}", _configConnectionString }
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configuration)
            .Build();
    }

    [Fact]
    public void OnAddDatabase_IfInMemoryFlag_IsEnabled_DatabaseIsInMemory()
    {
        //// Arrange
        var serviceCollection = new ServiceCollection();
        var fakeConfiguration = FakeInMemoryConfiguration;
        
        //// Act
        var result = ServiceCollectionExtensions.AddDatabase(serviceCollection, fakeConfiguration);

        //// Assert
        var provider = result.BuildServiceProvider();
        var context = provider.GetRequiredService<ApplicationDbContext>();

        context.Database.IsInMemory().Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("False")]
    [InlineData("Invalid_Bool")]
    public void OnAddDatabase_IfInMemoryFlag_IsDisabled_DatabaseIsExternal(string? inMemoryFlagValue)
    {
        //// Arrange
        var serviceCollection = new ServiceCollection();
        var fakeConfiguration = FakeExternalDatabaseConfiguration(inMemoryFlagValue);
        
        //// Act
        var result = ServiceCollectionExtensions.AddDatabase(serviceCollection, fakeConfiguration);

        //// Assert
        var provider = result.BuildServiceProvider();
        var context = provider.GetRequiredService<ApplicationDbContext>();

        context.Database.IsNpgsql().Should().BeTrue();
    }

    [Fact]
    public void OnAddDatabase_EnvironmentVariable_IsUsed_IfConnectionStringIsNotProvided()
    {
        //// Arrange
        var serviceCollection = new ServiceCollection();
        var fakeConfiguration = FakeExternalDatabaseConfiguration(null);
        Environment.SetEnvironmentVariable(
            Constants.ConnectionStringEnvironmentKey,
            _envConnectionString);
        
        //// Act
        var result = ServiceCollectionExtensions.AddDatabase(serviceCollection, fakeConfiguration);

        //// Assert
        var provider = result.BuildServiceProvider();
        var context = provider.GetRequiredService<ApplicationDbContext>();

        context.Database.GetConnectionString().Should().Be(_envConnectionString);
    }

    [Fact]
    public void OnAddDatabase_ConfigurationConnectionString_IsUsed_IfEnvironmentVariableIsNotProvided()
    {
        //// Arrange
        var serviceCollection = new ServiceCollection();
        var fakeConfiguration = FakeExternalDatabaseConfiguration(null);
        
        //// Act
        var result = ServiceCollectionExtensions.AddDatabase(serviceCollection, fakeConfiguration);

        //// Assert
        var provider = result.BuildServiceProvider();
        var context = provider.GetRequiredService<ApplicationDbContext>();

        context.Database.GetConnectionString().Should().Be(_configConnectionString);
    }
}