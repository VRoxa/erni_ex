using ERNI.Entities;
using Mapster;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using Xunit.Abstractions;

namespace ERNI.Core.Tests;

public class OrdersServiceTests(ITestOutputHelper testOutputHelper)
{
    private static IRepository<Order> FakeRepository => A.Fake<IRepository<Order>>();

    private ILogger<OrdersService> FakeLogger => Mogger<OrdersService>.Create(testOutputHelper);

    private static IClock FakeClock(DateTime? fakeTime = default)
    {
        var fakeClock = A.Fake<IClock>();
        A.CallTo(() => fakeClock.Now).Returns(fakeTime ?? DateTime.UtcNow);
        return fakeClock;
    }

    [Fact]
    public async Task OnAdd_Order_IsAdded()
    {
        var dateTime = DateTime.UtcNow;

        //// Arrange
        var fakeRepository = FakeRepository;
        var sut = new OrdersService(fakeRepository, FakeClock(dateTime), FakeLogger);

        //// Act
        var orderDto = new Faker<OrderDto>()
            .RuleFor(x => x.UserId, f => f.Random.Guid().ToString())
            .RuleFor(x => x.Amount, f => f.Random.Double())
            .Generate();

        var result = await sut.AddAsync(orderDto, CancellationToken.None);

        //// Assert
        A.CallTo(() => fakeRepository.Add(
                An<Order>.That.Matches(x => x.UserId.ToString() == orderDto.UserId &&
                    x.Amount == orderDto.Amount &&
                    x.CreatedAt == dateTime)))
            .MustHaveHappenedOnceExactly()
            .Then(
                A.CallTo(() => fakeRepository.SaveAsync(A<CancellationToken>._))
                .MustHaveHappenedOnceExactly());

        result.Id.Should().NotBeEmpty();
        result.UserId.Should().Be(orderDto.UserId);
        result.Amount.Should().Be(orderDto.Amount);
        result.CreatedAt.Should().Be(dateTime);
    }

    [Fact]
    public async Task OnGetFromUser_Orders_AreMapped()
    {
        var ids = Enumerable.Range(0, 4).Select(_ => Guid.NewGuid()).ToArray();
        var selectedId = ids.First();

        //// Arrange
        var orders = new Faker<Order>()
            .RuleFor(x => x.Id, f => f.Random.Guid())
            .RuleFor(x => x.UserId, f => f.PickRandom(ids))
            .RuleFor(x => x.Amount, f => f.Random.Double())
            .RuleFor(x => x.CreatedAt, f => f.Date.Past())
            .Generate(10);

        var fakeRepository = FakeRepository;
        A.CallTo(() => fakeRepository.Get(A<Expression<Func<Order, bool>>>.Ignored))
            .Returns(orders.Where(x => x.UserId == selectedId).AsAsyncQueryable());

        var sut = new OrdersService(fakeRepository, FakeClock(), FakeLogger);

        //// Act
        var result = await sut.GetFromUserAsync(
            selectedId.ToString(),
            CancellationToken.None);

        //// Assert
        var expectedOrders = orders.Where(x => x.UserId == selectedId);
        result.Should().BeEquivalentTo(expectedOrders.Select(x => x.Adapt<OrderDto>()));
    }

    [Fact]
    public async Task OnUpdate_Order_IsUpdated()
    {
        //// Arrange
        var originalOrder = new Faker<Order>()
            .RuleFor(x => x.Id, f => f.Random.Guid())
            .RuleFor(x => x.UserId, f => f.Random.Guid())
            .RuleFor(x => x.Amount, f => f.Random.Double())
            .RuleFor(x => x.CreatedAt, f => f.Date.Past())
            .Generate();

        var orderDto = new Faker<OrderDto>()
            .RuleFor(x => x.Id, f => f.Random.Guid().ToString())
            .RuleFor(x => x.Amount, f => f.Random.Double())
            .Generate();

        var fakeRepository = FakeRepository;
        A.CallTo(() => fakeRepository.FindAsync(
                A<CancellationToken>._,
                A<Guid>.That.IsEqualTo(Guid.Parse(orderDto.Id!))))
            .Returns(originalOrder);

        var sut = new OrdersService(fakeRepository, FakeClock(), FakeLogger);

        //// Act
        var result = await sut.UpdateAsync(orderDto, CancellationToken.None);

        //// Assert
        A.CallTo(() => fakeRepository.Update(
                An<Order>.That.Matches(x => x.Id == originalOrder.Id && x.Amount == orderDto.Amount)))
            .MustHaveHappenedOnceExactly()
            .Then(
                A.CallTo(() => fakeRepository.SaveAsync(A<CancellationToken>._))
                .MustHaveHappenedOnceExactly());
        
        result.Id.Should().Be(originalOrder.Id.ToString());
        result.Amount.Should().Be(orderDto.Amount);
        result.CreatedAt.Should().Be(originalOrder.CreatedAt);
    }

    [Fact]
    public async Task OnUpdate_IfOrderIsNotfound_ThrowsException()
    {
        //// Arrange
        var orderDto = new Faker<OrderDto>()
            .RuleFor(x => x.Id, f => f.Random.Guid().ToString())
            .Generate();

        var fakeRepository = FakeRepository;
        A.CallTo(() => fakeRepository.FindAsync(A<CancellationToken>._, A<Guid>.That.IsEqualTo(Guid.Parse(orderDto.Id!))))
            .Returns(null);

        var sut = new OrdersService(fakeRepository, FakeClock(), FakeLogger);

        //// Act
        var ex = await Record.ExceptionAsync(() => sut.UpdateAsync(orderDto, CancellationToken.None));

        //// Assert
        ex.Should().BeOfType<OrderNotFoundException>();
    }

    [Fact]
    public async Task OnRemove_Order_IsRemoved()
    {
        //// Arrange
        var originalOrder = new Faker<Order>()
            .RuleFor(x => x.Id, f => f.Random.Guid())
            .RuleFor(x => x.UserId, f => f.Random.Guid())
            .RuleFor(x => x.Amount, f => f.Random.Double())
            .RuleFor(x => x.CreatedAt, f => f.Date.Past())
            .Generate();

        var fakeRepository = FakeRepository;
        A.CallTo(() => fakeRepository.FindAsync(A<CancellationToken>._, A<Guid>.That.IsEqualTo(originalOrder.Id)))
            .Returns(originalOrder);

        var sut = new OrdersService(fakeRepository, FakeClock(), FakeLogger);

        //// Act
        await sut.RemoveAsync(originalOrder.Id.ToString(), CancellationToken.None);

        //// Assert
        A.CallTo(() => fakeRepository.Remove(An<Order>.That.IsEqualTo(originalOrder)))
            .MustHaveHappenedOnceExactly()
            .Then(
                A.CallTo(() => fakeRepository.SaveAsync(A<CancellationToken>._))
                .MustHaveHappenedOnceExactly());
    }

    [Fact]
    public async Task OnRemove_IfOrderIsNotFound_ThrowsException()
    {
        //// Arrange
        var originalOrder = new Faker<Order>()
            .RuleFor(x => x.Id, f => f.Random.Guid())
            .RuleFor(x => x.UserId, f => f.Random.Guid())
            .RuleFor(x => x.Amount, f => f.Random.Double())
            .RuleFor(x => x.CreatedAt, f => f.Date.Past())
            .Generate();

        var fakeRepository = FakeRepository;
        A.CallTo(() => fakeRepository.FindAsync(A<CancellationToken>._, A<Guid>.That.IsEqualTo(originalOrder.Id)))
            .Returns(null);

        var sut = new OrdersService(fakeRepository, FakeClock(), FakeLogger);

        //// Act
        var ex = await Record.ExceptionAsync(() => sut.RemoveAsync(originalOrder.Id.ToString(), CancellationToken.None));

        //// Assert
        ex.Should().BeOfType<OrderNotFoundException>();
    }
}
