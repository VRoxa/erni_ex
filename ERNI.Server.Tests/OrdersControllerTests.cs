using ERNI.Core;
using ERNI.Server.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace ERNI.Server.Tests;

public class OrdersControllerTests
{
    private static IOrdersService FakeService => A.Fake<IOrdersService>();

    [Fact]
    public async Task OnGetFromUser_Orders_AreReturned()
    {
        var userId = "Testing";

        //// Arrange
        var orders = A.CollectionOfDummy<OrderDto>(5);
        
        var fakeService = FakeService;
        A.CallTo(() => fakeService.GetFromUserAsync(A<string>.That.IsEqualTo(userId), A<CancellationToken>._))
            .Returns(orders);

        var sut = new OrdersController();

        //// Act
        var result = await sut.GetFromUser(userId, fakeService, CancellationToken.None);

        //// Assert
        result.Should().BeEquivalentTo(orders);
    }

    [Fact]
    public async Task OnCreate_Order_IsAdded()
    {
        var orderId = "Testing";

        //// Arrange
        var orderDto = A.Dummy<OrderDto>();
        var fakeService = FakeService;
        A.CallTo(() => fakeService.AddAsync(A<OrderDto>.That.IsSameAs(orderDto), A<CancellationToken>._))
            .Returns(orderDto with { Id = orderId });

        var sut = new OrdersController();

        //// Act
        var result = await sut.Create(orderDto, fakeService, CancellationToken.None);

        //// Assert
        result.Should().BeOfType<CreatedResult>().Which.Location.Should().Be(orderId);
    }

    [Fact]
    public async Task OnUpdate_Order_IsUpdated()
    {
        //// Arrange
        var orderDto = A.Dummy<OrderDto>();
        var fakeService = FakeService;
        A.CallTo(() => fakeService.UpdateAsync(A<OrderDto>.That.IsSameAs(orderDto), A<CancellationToken>._))
            .Returns(orderDto);

        var sut = new OrdersController();

        //// Act
        var result = await sut.Update(orderDto, fakeService, CancellationToken.None);

        //// Assert
        result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeSameAs(orderDto);
    }

    [Fact]
    public async Task OnRemove_Order_IsRemoved()
    {
        //// Arrange
        var sut = new OrdersController();

        //// Act
        var result = await sut.Remove("Testing", FakeService, CancellationToken.None);

        //// Assert
        result.Should().BeOfType<OkResult>();
    }
}
