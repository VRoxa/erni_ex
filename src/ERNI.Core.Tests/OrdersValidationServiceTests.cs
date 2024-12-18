using ERNI.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERNI.Core.Tests;

public class OrdersValidationServiceTests
{
    private static IOrdersService FakeService => A.Fake<IOrdersService>();

    private static IValidatorProvider FakePassingValidatorProvider
    {
        get
        {
            var validator = new InlineValidator<OrderDto>();
            validator.RuleFor(x => x).Must(x => true);

            var provider = A.Fake<IValidatorProvider>();
            A.CallTo(() => provider.Provide<OrderDto>(A<IValidatorProvider.ValidationType>._)).Returns(validator);
            return provider;
        }
    }

    private static IValidatorProvider FakeFailingValidatorProvider
    {
        get
        {
            var validator = new InlineValidator<OrderDto>();
            validator.RuleFor(x => x).Must(x => false);

            var provider = A.Fake<IValidatorProvider>();
            A.CallTo(() => provider.Provide<OrderDto>(A<IValidatorProvider.ValidationType>._)).Returns(validator);
            return provider;
        }
    }

    [Fact]
    public async Task OnAddAsync_ValidOrderDto_InvokesService()
    {
        var orderDto = A.Dummy<OrderDto>();

        //// Arrange
        var fakeService = FakeService;
        A.CallTo(() => fakeService.AddAsync(An<OrderDto>.That.IsSameAs(orderDto), A<CancellationToken>._))
            .Returns(orderDto);

        var sut = new OrdersValidationService(FakePassingValidatorProvider, fakeService);

        //// Act
        var result = await sut.AddAsync(orderDto, CancellationToken.None);

        //// Assert
        result.Should().BeSameAs(orderDto);
    }

    [Fact]
    public async Task OnAddAsync_InvalidOrderDto_ThrowsInvalidRequestException()
    {
        var orderDto = A.Dummy<OrderDto>();

        //// Arrange
        var fakeService = FakeService;
        var sut = new OrdersValidationService(FakeFailingValidatorProvider, fakeService);

        //// Act
        var ex = await Record.ExceptionAsync(() => sut.AddAsync(orderDto, CancellationToken.None));

        //// Assert
        ex.Should().BeOfType<InvalidRequestException>();
    }

    [Fact]
    public async Task OnGetFromUserAsync_ValidUserId_InvokesService()
    {
        var userId = Guid.NewGuid().ToString();
        var orderDtos = A.CollectionOfDummy<OrderDto>(3).ToList();

        //// Arrange
        var fakeService = FakeService;
        A.CallTo(() => fakeService.GetFromUserAsync(userId, A<CancellationToken>._))
            .Returns(orderDtos);

        var sut = new OrdersValidationService(FakePassingValidatorProvider, fakeService);

        //// Act
        var result = await sut.GetFromUserAsync(userId, CancellationToken.None);

        //// Assert
        result.Should().BeEquivalentTo(orderDtos);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Malformatted")]
    public async Task OnGetFromUserAsync_InvalidUserId_ThrowsInvalidRequestException(string? userId)
    {
        //// Arrange
        var fakeService = FakeService;
        var sut = new OrdersValidationService(FakePassingValidatorProvider, fakeService);

        //// Act
        var ex = await Record.ExceptionAsync(() => sut.GetFromUserAsync(userId!, CancellationToken.None));

        //// Assert
        ex.Should().BeOfType<InvalidRequestException>();
    }

    [Fact]
    public async Task OnRemoveAsync_ValidOrderId_InvokesService()
    {
        var orderId = Guid.NewGuid().ToString();

        //// Arrange
        var fakeService = FakeService;
        var sut = new OrdersValidationService(FakePassingValidatorProvider, fakeService);

        //// Act
        await sut.RemoveAsync(orderId, CancellationToken.None);

        //// Assert
        A.CallTo(() => fakeService.RemoveAsync(orderId, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Malformatted")]
    public async Task OnRemoveAsync_InvalidOrderId_ThrowsInvalidRequestException(string? orderId)
    {
        //// Arrange
        var fakeService = FakeService;
        var sut = new OrdersValidationService(FakePassingValidatorProvider, fakeService);

        //// Act
        var ex = await Record.ExceptionAsync(() => sut.RemoveAsync(orderId!, CancellationToken.None));

        //// Assert
        ex.Should().BeOfType<InvalidRequestException>();
    }

    [Fact]
    public async Task OnUpdateAsync_ValidOrderDto_InvokesService()
    {
       var orderDto = A.Dummy<OrderDto>();

        //// Arrange
        var fakeService = FakeService;
        A.CallTo(() => fakeService.UpdateAsync(An<OrderDto>.That.IsSameAs(orderDto), A<CancellationToken>._))
            .Returns(orderDto);

        var sut = new OrdersValidationService(FakePassingValidatorProvider, fakeService);

        //// Act
        var result = await sut.UpdateAsync(orderDto, CancellationToken.None);

        //// Assert
        result.Should().BeSameAs(orderDto);
    }

    [Fact]
    public async Task OnUpdateAsync_InvalidOrderDto_ThrowsInvalidRequestException()
    {
        var orderDto = A.Dummy<OrderDto>();

        //// Arrange
        var fakeService = FakeService;
        var sut = new OrdersValidationService(FakeFailingValidatorProvider, fakeService);

        //// Act
        var ex = await Record.ExceptionAsync(() => sut.UpdateAsync(orderDto, CancellationToken.None));

        //// Assert
        ex.Should().BeOfType<InvalidRequestException>();
    }
}
