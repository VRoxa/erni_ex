using ERNI.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ERNI.Core;

internal sealed class OrdersService : IOrdersService
{
    private readonly IRepository<Order> _repository;
    private readonly IClock _clock;
    private readonly ILogger<OrdersService> _logger;

    public OrdersService(
        IRepository<Order> repository,
        IClock clock,
        ILogger<OrdersService> logger)
    {
        _repository = repository;
        _clock = clock;
        _logger = logger;
    }

    public async Task<OrderDto> AddAsync(OrderDto orderDto, CancellationToken token)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = Guid.Parse(orderDto.UserId),
            Amount = orderDto.Amount,
            CreatedAt = _clock.Now,
        };

        _repository.Add(order);
        await _repository.SaveAsync(token);

        _logger.LogInformation("Created order '{OrderId}' for user '{UserId}'", order.Id, orderDto.UserId);
        return order.Adapt<OrderDto>();
    }

    public async Task<IList<OrderDto>> GetFromUserAsync(string userId, CancellationToken token)
    {
        var orders = await _repository
            .Get(x => x.UserId == Guid.Parse(userId))
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => x.Adapt<OrderDto>())
            .ToListAsync(token);

        return orders;
    }

    public async Task<OrderDto> UpdateAsync(OrderDto orderDto, CancellationToken token)
    {
        var order = await _repository.FindAsync(token, Guid.Parse(orderDto.Id!));
        if (order is null)
        {
            _logger.LogError("Could not update order by ID '{Id}'. Order not found", orderDto.Id);
            throw new OrderNotFoundException(orderDto.Id!);
        }

        var updatedOrder = order with
        {
            Amount = orderDto.Amount
        };

        _repository.Update(updatedOrder);
        await _repository.SaveAsync(token);

        _logger.LogInformation("Order with ID '{Id}' has been updated", order.Id);
        return updatedOrder.Adapt<OrderDto>();
    }

    public async Task RemoveAsync(string orderId, CancellationToken token)
    {
        var order = await _repository.FindAsync(token, Guid.Parse(orderId));
        if (order is null)
        {
            _logger.LogError("Could not remove order by ID '{Id}'. Order not found", orderId);
            throw new OrderNotFoundException(orderId);
        }

        _repository.Remove(order);
        await _repository.SaveAsync(token);

        _logger.LogInformation("Removed order by ID '{Id}'", order.Id);
    }
}
