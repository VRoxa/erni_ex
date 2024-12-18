namespace ERNI.Core;

public interface IOrdersService
{
    Task<IList<OrderDto>> GetFromUserAsync(string userId, CancellationToken token);

    Task<OrderDto> AddAsync(OrderDto orderDto, CancellationToken token);

    Task<OrderDto> UpdateAsync(OrderDto orderDto, CancellationToken token);

    Task RemoveAsync(string orderId, CancellationToken token);
}
