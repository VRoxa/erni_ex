namespace ERNI.Entities;

public record User : IEntity
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public string? Email { get; init; }

    public Order[] Orders { get; init; } = [];
}
