using ERNI.Core;
using Microsoft.AspNetCore.Mvc;

namespace ERNI.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class OrdersController : ControllerBase
{
    [HttpGet("{userId}")]
    public async Task<IList<OrderDto>> GetFromUser(
        [FromRoute] string userId,
        [FromServices] IOrdersService ordersService,
        CancellationToken token)
    {
        var orders = await ordersService.GetFromUserAsync(userId, token);
        return orders;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] OrderDto orderDto,
        [FromServices] IOrdersService ordersService,
        CancellationToken token)
    {
        var createdOrder = await ordersService.AddAsync(orderDto, token);
        return Created(createdOrder.Id!.ToString(), createdOrder);
    }

    [HttpPatch]
    public async Task<IActionResult> Update(
        [FromBody] OrderDto orderDto,
        [FromServices] IOrdersService ordersService,
        CancellationToken token)
    {
        var updatedUser = await ordersService.UpdateAsync(orderDto, token);
        return Ok(updatedUser);
    }

    [HttpDelete("{orderId}")]
    public async Task<IActionResult> Remove(
        [FromRoute] string orderId,
        [FromServices] IOrdersService ordersService,
        CancellationToken token)
    {
        await ordersService.RemoveAsync(orderId, token);
        return Ok();
    }
}
