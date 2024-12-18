using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERNI.Core.Tests;

public class UsersValidationServiceTests
{
    private static IUsersService FakeService => A.Fake<IUsersService>();

    private static IValidatorProvider FakePassingValidatorProvider
    {
        get
        {
            var validator = new InlineValidator<UserDto>();
            validator.RuleFor(x => x).Must(x => true);

            var provider = A.Fake<IValidatorProvider>();
            A.CallTo(() => provider.Provide<UserDto>(A<IValidatorProvider.ValidationType>._)).Returns(validator);
            return provider;
        }
    }

    private static IValidatorProvider FakeFailingValidatorProvider
    {
        get
        {
            var validator = new InlineValidator<UserDto>();
            validator.RuleFor(x => x).Must(x => false);

            var provider = A.Fake<IValidatorProvider>();
            A.CallTo(() => provider.Provide<UserDto>(A<IValidatorProvider.ValidationType>._)).Returns(validator);
            return provider;
        }
    }

    [Fact]
    public async Task OnAddAsync_ValidUserDto_InvokesService()
    {
        var userDto = A.Dummy<UserDto>();

        //// Arrange
        var fakeService = FakeService;
        A.CallTo(() => fakeService.AddAsync(An<UserDto>.That.IsSameAs(userDto), A<CancellationToken>._))
            .Returns(userDto);

        var sut = new UsersValidationService(FakePassingValidatorProvider, fakeService);

        //// Act
        var result = await sut.AddAsync(userDto, CancellationToken.None);

        //// Assert
        result.Should().BeSameAs(userDto);
    }

    [Fact]
    public async Task OnAddAsync_InvalidUserDto_ThrowsException()
    {
        var userDto = A.Dummy<UserDto>();

        //// Arrange
        var fakeService = FakeService;
        A.CallTo(() => fakeService.AddAsync(An<UserDto>.That.IsSameAs(userDto), A<CancellationToken>._))
            .Returns(userDto);

        var sut = new UsersValidationService(FakeFailingValidatorProvider, fakeService);

        //// Act
        var ex = await Record.ExceptionAsync(() => sut.AddAsync(userDto, CancellationToken.None));

        //// Assert
        ex.Should().BeOfType<InvalidRequestException>();
    }

    [Fact]
    public async Task OnGetAllAsync_InvokesService()
    {
        var userDtos = A.CollectionOfDummy<UserDto>(3);

        //// Arrange
        var fakeService = FakeService;
        A.CallTo(() => fakeService.GetAllAsync(A<CancellationToken>._))
            .Returns(userDtos);

        var sut = new UsersValidationService(FakePassingValidatorProvider, fakeService);

        //// Act
        var result = await sut.GetAllAsync(CancellationToken.None);

        //// Assert
        result.Should().BeEquivalentTo(userDtos);
    }

    [Fact]
    public async Task OnRemoveAsync_ValidUserId_InvokesService()
    {
        var userId = Guid.NewGuid().ToString();

        //// Arrange
        var fakeService = FakeService;
        var sut = new UsersValidationService(FakePassingValidatorProvider, fakeService);

        //// Act
        await sut.RemoveAsync(userId, CancellationToken.None);

        //// Assert
        A.CallTo(() => fakeService.RemoveAsync(userId, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Malformatted")]
    public async Task OnRemoveAsync_InvalidUserId_ThrowsException(string? userId)
    {
        //// Arrange
        var fakeService = FakeService;
        var sut = new UsersValidationService(FakePassingValidatorProvider, fakeService);

        //// Act
        var ex = await Record.ExceptionAsync(() => sut.RemoveAsync(userId!, CancellationToken.None));

        //// Assert
        ex.Should().BeOfType<InvalidRequestException>();
    }

    [Fact]
    public async Task OnUpdateAsync_ValidUserDto_InvokesService()
    {
        var userDto = A.Dummy<UserDto>();

        //// Arrange
        var fakeService = FakeService;
        A.CallTo(() => fakeService.UpdateAsync(An<UserDto>.That.IsSameAs(userDto), A<CancellationToken>._))
            .Returns(userDto);

        var sut = new UsersValidationService(FakePassingValidatorProvider, fakeService);

        //// Act
        var result = await sut.UpdateAsync(userDto, CancellationToken.None);

        //// Assert
        result.Should().BeSameAs(userDto);
    }

    [Fact]
    public async Task OnUpdateAsync_InvalidUserDto_ThrowsException()
    {
        var userDto = A.Dummy<UserDto>();

        //// Arrange
        var fakeService = FakeService;
        A.CallTo(() => fakeService.UpdateAsync(An<UserDto>.That.IsSameAs(userDto), A<CancellationToken>._))
            .Returns(userDto);

        var sut = new UsersValidationService(FakeFailingValidatorProvider, fakeService);

        //// Act
        var ex = await Record.ExceptionAsync(() => sut.UpdateAsync(userDto, CancellationToken.None));

        //// Assert
        ex.Should().BeOfType<InvalidRequestException>();
    }
}
