namespace ERNI.Core;

public sealed record UserDto
{
    public string? Id { get; init; }

    public required string Name { get; init; }

    public string? Email { get; init; }

    public OrderDto[] Orders { get; init; } = [];
}
