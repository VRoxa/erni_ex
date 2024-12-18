using ERNI.Entities;

namespace ERNI.Core;

public interface IUsersService
{
    Task<IList<UserDto>> GetAllAsync(CancellationToken token);

    Task<UserDto> AddAsync(UserDto userDto, CancellationToken token);

    Task<UserDto> UpdateAsync(UserDto userDto, CancellationToken token);

    Task RemoveAsync(string userId, CancellationToken token);
}
