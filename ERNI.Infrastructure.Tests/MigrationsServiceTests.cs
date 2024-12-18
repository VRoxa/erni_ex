using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace ERNI.Infrastructure.Tests;

public class MigrationsServiceTests(ITestOutputHelper testOutputHelper)
{
    private static IDatabaseFacadeShim FakeFacade => A.Fake<IDatabaseFacadeShim>();

    private static ApplicationDbContext FakeContext => new(new DbContextOptions<ApplicationDbContext>());

    private ILogger<MigrationsService> FakeLogger => Mogger<MigrationsService>.Create(testOutputHelper);

    private static IDatabaseFacadeProvider FakeFacadeProvider => A.Fake<IDatabaseFacadeProvider>();

    [Fact]
    public async void OnMigrate_DatabaseMigration_IsInvoked()
    {
        //// Arrange
        var facade = FakeFacade;
        var context = FakeContext;
        var facadeProvider = FakeFacadeProvider;

        A.CallTo(() => facadeProvider.ProvideFor(A<DbContext>.That.IsSameAs(context)))
            .Returns(facade);

        var sut = new MigrationsService(context, facadeProvider, FakeLogger);

        //// Act
        await sut.MigrateAsync(CancellationToken.None);

        //// Assert
        A.CallTo(() => facade.MigrateAsync(A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
}
