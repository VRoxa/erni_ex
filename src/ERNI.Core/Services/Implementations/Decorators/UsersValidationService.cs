namespace ERNI.Core;

internal sealed class UsersValidationService : IUsersService
{
    private readonly IValidatorProvider _validatorProvider;
    private readonly IUsersService _decorated;

    public UsersValidationService(
        IValidatorProvider validatorProvider,
        IUsersService decorated)
    {
        _validatorProvider = validatorProvider;
        _decorated = decorated;
    }

    public Task<UserDto> AddAsync(UserDto userDto, CancellationToken token)
    {
        Validate(userDto, IValidatorProvider.ValidationType.WithoutId);
        return _decorated.AddAsync(userDto, token);
    }

    public Task<IList<UserDto>> GetAllAsync(CancellationToken token)
    {
        return _decorated.GetAllAsync(token);
    }

    public Task RemoveAsync(string userId, CancellationToken token)
    {
        if (!Guid.TryParse(userId, out _))
        {
            throw new InvalidRequestException("Malformatted User ID GUID");
        }

        return _decorated.RemoveAsync(userId, token);
    }

    public Task<UserDto> UpdateAsync(UserDto userDto, CancellationToken token)
    {
        Validate(userDto, IValidatorProvider.ValidationType.WithId);
        return _decorated.UpdateAsync(userDto, token);
    }

    private void Validate(UserDto dto, IValidatorProvider.ValidationType type)
    {
        var validator = _validatorProvider.Provide<UserDto>(type);
        var validationResult = validator.Validate(dto);
        if (!validationResult.IsValid)
        {
            var message = string.Join('\n', validationResult.Errors.Select(x => x.ErrorMessage));
            throw new InvalidRequestException(message);
        }
    }
}