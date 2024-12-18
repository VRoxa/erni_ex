using ERNI.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ERNI.Core;

internal sealed class UsersService : IUsersService
{
    private readonly IRepository<User> _repository;
    private readonly ILogger<UsersService> _logger;

    public UsersService(
        IRepository<User> repository,
        ILogger<UsersService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IList<UserDto>> GetAllAsync(CancellationToken token)
    {
        var users = await _repository
            .GetAll()
            .Select(x => x.Adapt<UserDto>())
            .ToListAsync(token);

        return users;
    }

    public async Task<UserDto> AddAsync(UserDto userDto, CancellationToken token)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = userDto.Name,
            Email = userDto.Email,
        };

        _repository.Add(user);
        await _repository.SaveAsync(token);

        _logger.LogInformation("Created user '{Name}' with ID '{Id}'", user.Name, user.Id);
        return user.Adapt<UserDto>();
    }

    public async Task<UserDto> UpdateAsync(UserDto userDto, CancellationToken token)
    {
        var user = await _repository.FindAsync(token, Guid.Parse(userDto.Id!));
        if (user is null)
        {
            _logger.LogError("Could not update user by ID '{Id}'. User not found", userDto.Id);
            throw new UserNotFoundException(userDto.Id!);
        }

        var updatedUser = user with
        {
            Name = userDto.Name,
            Email = userDto.Email
        };

        _repository.Update(updatedUser);
        await _repository.SaveAsync(token);

        _logger.LogInformation("User with ID '{Id}' has been updated", user.Id);
        return updatedUser.Adapt<UserDto>();
    }

    public async Task RemoveAsync(string userId, CancellationToken token)
    {
        var user = await _repository.FindAsync(token, Guid.Parse(userId));
        if (user is null)
        {
            _logger.LogError("Could not remove user by ID '{Id}'. User not found", userId);
            throw new UserNotFoundException(userId);
        }

        _repository.Remove(user);
        await _repository.SaveAsync(token);

        _logger.LogInformation("Removed user '{Name}' by ID '{Id}'", user.Name, user.Id);
    }
}
