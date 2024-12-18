using ERNI.Core;
using ERNI.Server.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace ERNI.Server.Tests;

public class UsersControllerTests
{
    private static IUsersService FakeService => A.Fake<IUsersService>();

    [Fact]
    public async Task OnGetAll_Users_AreReturned()
    {
        //// Arrange
        var users = A.CollectionOfDummy<UserDto>(5);
        
        var fakeService = FakeService;
        A.CallTo(() => fakeService.GetAllAsync(A<CancellationToken>._))
            .Returns(users);

        var sut = new UsersController();

        //// Act
        var result = await sut.GetAll(fakeService, CancellationToken.None);

        //// Assert
        result.Should().BeEquivalentTo(users);
    }

    [Fact]
    public async Task OnCreate_User_IsAdded()
    {
        var userId = "Testing";

        //// Arrange
        var userDto = A.Dummy<UserDto>();
        var fakeService = FakeService;
        A.CallTo(() => fakeService.AddAsync(A<UserDto>.That.IsSameAs(userDto), A<CancellationToken>._))
            .Returns(userDto with { Id = userId });

        var sut = new UsersController();

        //// Act
        var result = await sut.Create(userDto, fakeService, CancellationToken.None);

        //// Assert
        result.Should().BeOfType<CreatedResult>().Which.Location.Should().Be(userId);
    }

    [Fact]
    public async Task OnUpdate_User_IsUpdated()
    {
        //// Arrange
        var userDto = A.Dummy<UserDto>();
        var fakeService = FakeService;
        A.CallTo(() => fakeService.UpdateAsync(A<UserDto>.That.IsSameAs(userDto), A<CancellationToken>._))
            .Returns(userDto);

        var sut = new UsersController();

        //// Act
        var result = await sut.Update(userDto, fakeService, CancellationToken.None);

        //// Assert
        result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeSameAs(userDto);
    }

    [Fact]
    public async Task OnRemove_User_IsRemoved()
    {
        //// Arrange
        var sut = new UsersController();

        //// Act
        var result = await sut.Remove("Testing", FakeService, CancellationToken.None);

        //// Assert
        result.Should().BeOfType<OkResult>();
    }
}
