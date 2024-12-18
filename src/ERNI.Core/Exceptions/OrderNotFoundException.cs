
namespace ERNI.Core;

public sealed class OrderNotFoundException : Exception
{
    public OrderNotFoundException(string id)
        : this(id, null)
    {
    }

    public OrderNotFoundException(string id, Exception? innerException)
        : base($"User not found by ID '{id}'", innerException)
    {

    }
}
