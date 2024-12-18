using ERNI.Infrastructure;
using ERNI.Server.Services;

namespace ERNI.Server.Tests;

public class MigrationsHostedServiceTests
{
    [Fact]
    public async void OnStart_MigrationService_IsInvoked()
    {
        //// Arrange
        var fakeMigrationService = A.Fake<IMigrationsService>();
        var sut = new MigrationsHostedService(fakeMigrationService);

        //// Act
        await sut.StartAsync(CancellationToken.None);

        //// Assert
        A.CallTo(() => fakeMigrationService.MigrateAsync(A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
}