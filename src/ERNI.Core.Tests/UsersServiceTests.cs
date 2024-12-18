using ERNI.Entities;
using Mapster;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace ERNI.Core.Tests;

public class UsersServiceTests(ITestOutputHelper testOutputHelper)
{
    private static IRepository<User> FakeRepository => A.Fake<IRepository<User>>();

    private ILogger<UsersService> FakeLogger => Mogger<UsersService>.Create(testOutputHelper);

    [Fact]
    public async Task OnGetAll_AllUsers_AreMapped()
    {
        //// Arrange
        var users = new Faker<User>()
            .RuleFor(x => x.Id, f => f.Random.Guid())
            .RuleFor(x => x.Name, f => f.Person.FullName)
            .RuleFor(x => x.Email, (f, x) => f.Internet.Email(x.Name))
            .Generate(1);

        var fakeRepository = FakeRepository;
        A.CallTo(() => fakeRepository.GetAll()).Returns(users.AsAsyncQueryable());

        var sut = new UsersService(fakeRepository, FakeLogger);

        //// Act
        var result = await sut.GetAllAsync(CancellationToken.None);

        //// Assert
        result.Should().BeEquivalentTo(users.Select(x => x.Adapt<UserDto>()));
    }

    [Fact]
    public async Task OnAdd_User_IsAdded()
    {
        //// Arrange
        var fakeRepository = FakeRepository;
        var sut = new UsersService(fakeRepository, FakeLogger);

        //// Act
        var userDto = new Faker<UserDto>()
            .RuleFor(x => x.Name, f => f.Person.FullName)
            .RuleFor(x => x.Email, (f, x) => f.Internet.Email(x.Name))
            .Generate();

        var result = await sut.AddAsync(userDto, CancellationToken.None);

        //// Assert
        A.CallTo(() => fakeRepository.Add(
                A<User>.That.Matches(x => x.Name == userDto.Name && x.Email == userDto.Email)))
            .MustHaveHappenedOnceExactly()
            .Then(
                A.CallTo(() => fakeRepository.SaveAsync(A<CancellationToken>._))
                .MustHaveHappenedOnceExactly());

        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be(userDto.Name);
        result.Email.Should().Be(userDto.Email);
        result.Orders.Should().BeEmpty();
    }

    [Fact]
    public async Task OnUpdate_User_IsUpdated()
    {
        //// Arrange
        var originalUser = new Faker<User>()
            .RuleFor(x => x.Id, f => f.Random.Guid())
            .RuleFor(x => x.Name, f => f.Person.FullName)
            .RuleFor(x => x.Email, (f, x) => f.Internet.Email(x.Name))
            .Generate();

        var userDto = new Faker<UserDto>()
            .RuleFor(x => x.Id, f => f.Random.Guid().ToString())
            .RuleFor(x => x.Name, f => f.Person.FullName)
            .RuleFor(x => x.Email, (f, x) => f.Internet.Email(x.Name))
            .Generate();

        var fakeRepository = FakeRepository;
        A.CallTo(() => fakeRepository.FindAsync(A<CancellationToken>._, A<Guid>.That.IsEqualTo(Guid.Parse(userDto.Id!))))
            .Returns(originalUser);

        var sut = new UsersService(fakeRepository, FakeLogger);

        //// Act
        var result = await sut.UpdateAsync(userDto, CancellationToken.None);

        //// Assert
        A.CallTo(() => fakeRepository.Update(
                A<User>.That.Matches(x => x.Id == originalUser.Id && x.Name == userDto.Name && x.Email == userDto.Email)))
            .MustHaveHappenedOnceExactly()
            .Then(
                A.CallTo(() => fakeRepository.SaveAsync(A<CancellationToken>._))
                .MustHaveHappenedOnceExactly());

        result.Id.Should().Be(originalUser.Id.ToString());
        result.Name.Should().Be(userDto.Name);
        result.Email.Should().Be(userDto.Email);
        result.Orders.Should().BeEquivalentTo(userDto.Orders);
    }

    [Fact]
    public async Task OnUpdate_IfUserIsNotFound_ThrowsException()
    {
        //// Arrange
        var userDto = new Faker<UserDto>()
            .RuleFor(x => x.Id, f => f.Random.Guid().ToString())
            .Generate();

        var fakeRepository = FakeRepository;
        A.CallTo(() => fakeRepository.FindAsync(A<CancellationToken>._, A<Guid>.That.IsEqualTo(Guid.Parse(userDto.Id!))))
            .Returns(null);

        var sut = new UsersService(fakeRepository, FakeLogger);

        //// Act
        var ex = await Record.ExceptionAsync(() => sut.UpdateAsync(userDto, CancellationToken.None));

        //// Assert
        ex.Should().BeOfType<UserNotFoundException>();
    }

    [Fact]
    public async Task OnRemove_User_IsRemoved()
    {
        //// Arrange
        var originalUser = new Faker<User>()
            .RuleFor(x => x.Id, f => f.Random.Guid())
            .RuleFor(x => x.Name, f => f.Person.FullName)
            .RuleFor(x => x.Email, (f, x) => f.Internet.Email(x.Name))
            .Generate();

        var fakeRepository = FakeRepository;
        A.CallTo(() => fakeRepository.FindAsync(A<CancellationToken>._, A<Guid>.That.IsEqualTo(originalUser.Id)))
            .Returns(originalUser);

        var sut = new UsersService(fakeRepository, FakeLogger);

        //// Act
        await sut.RemoveAsync(originalUser.Id.ToString(), CancellationToken.None);

        //// Assert
        A.CallTo(() => fakeRepository.Remove(A<User>.That.IsEqualTo(originalUser)))
            .MustHaveHappenedOnceExactly()
            .Then(
                A.CallTo(() => fakeRepository.SaveAsync(A<CancellationToken>._))
                .MustHaveHappenedOnceExactly());
    }

    [Fact]
    public async Task OnRemove_IfUserIsNotFound_ThrowsException()
    {
        //// Arrange
        var originalUser = new Faker<User>()
            .RuleFor(x => x.Id, f => f.Random.Guid())
            .RuleFor(x => x.Name, f => f.Person.FullName)
            .RuleFor(x => x.Email, (f, x) => f.Internet.Email(x.Name))
            .Generate();

        var fakeRepository = FakeRepository;
        A.CallTo(() => fakeRepository.FindAsync(A<CancellationToken>._, A<Guid>.That.IsEqualTo(originalUser.Id)))
            .Returns(null);

        var sut = new UsersService(fakeRepository, FakeLogger);

        //// Act
        var ex = await Record.ExceptionAsync(() => sut.RemoveAsync(originalUser.Id.ToString(), CancellationToken.None));

        //// Assert
        ex.Should().BeOfType<UserNotFoundException>();
    }
}