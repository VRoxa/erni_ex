using ERNI.Core;
using Microsoft.AspNetCore.Mvc;

namespace ERNI.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UsersController : ControllerBase
{
    [HttpGet]
    public async Task<IList<UserDto>> GetAll(
        [FromServices] IUsersService usersService,
        CancellationToken token)
    {
        var users = await usersService.GetAllAsync(token);
        return users;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] UserDto userDto,
        [FromServices] IUsersService usersService,
        CancellationToken token)
    {
        // TODO - Validate
        var createdUser = await usersService.AddAsync(userDto, token);
        return Created(createdUser.Id!.ToString(), createdUser);
    }

    [HttpPatch]
    public async Task<IActionResult> Update(
        [FromBody] UserDto userDto,
        [FromServices] IUsersService usersService,
        CancellationToken token)
    {
        var updatedUser = await usersService.UpdateAsync(userDto, token);
        return Ok(updatedUser);
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> Remove(
        [FromRoute] string userId,
        [FromServices] IUsersService usersService,
        CancellationToken token)
    {
        await usersService.RemoveAsync(userId, token);
        return Ok();
    }
}
