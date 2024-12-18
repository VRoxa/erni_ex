namespace ERNI.Core;

internal sealed class OrdersValidationService : IOrdersService
{
    private readonly IValidatorProvider _validatorProvider;
    private readonly IOrdersService _ordersService;

    public OrdersValidationService(
        IValidatorProvider validatorProvider,
        IOrdersService ordersService)
    {
        _validatorProvider = validatorProvider;
        _ordersService = ordersService;
    }

    public Task<OrderDto> AddAsync(OrderDto orderDto, CancellationToken token)
    {
        Validate(orderDto, IValidatorProvider.ValidationType.WithoutId);
        return _ordersService.AddAsync(orderDto, token);
    }

    public Task<IList<OrderDto>> GetFromUserAsync(string userId, CancellationToken token)
    {
        if (!Guid.TryParse(userId, out _))
        {
            throw new InvalidRequestException("Malformatted User ID GUID");
        }

        return _ordersService.GetFromUserAsync(userId, token);
    }

    public Task RemoveAsync(string orderId, CancellationToken token)
    {
        if (!Guid.TryParse(orderId, out _))
        {
            throw new InvalidRequestException("Malformatted Order ID GUID");
        }

        return _ordersService.RemoveAsync(orderId, token);
    }

    public Task<OrderDto> UpdateAsync(OrderDto orderDto, CancellationToken token)
    {
        Validate(orderDto, IValidatorProvider.ValidationType.WithId);
        return _ordersService.UpdateAsync(orderDto, token);
    }

    private void Validate(OrderDto dto, IValidatorProvider.ValidationType type)
    {
        var validator = _validatorProvider.Provide<OrderDto>(type);
        var validationResult = validator.Validate(dto);
        if (!validationResult.IsValid)
        {
            var message = string.Join('\n', validationResult.Errors.Select(x => x.ErrorMessage));
            throw new InvalidRequestException(message);
        }
    }
}
