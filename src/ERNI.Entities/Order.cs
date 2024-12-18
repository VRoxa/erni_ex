namespace ERNI.Entities;

public record Order : IEntity
{
    public required Guid Id { get; init; }

    public required DateTime CreatedAt { get; init; }

    public double Amount { get; init; }

    public required Guid UserId { get; init; }
}
