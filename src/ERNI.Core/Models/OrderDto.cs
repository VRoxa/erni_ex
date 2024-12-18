namespace ERNI.Core;

public sealed record OrderDto
{
    public string? Id { get; init; }

    public DateTime CreatedAt { get; init; }

    public double Amount { get; init; }

    public required string UserId { get; init; }
}

