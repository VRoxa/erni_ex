
namespace ERNI.Core;

public sealed class UserNotFoundException : Exception
{
    public UserNotFoundException(string id)
        : this(id, null)
    {
    }

    public UserNotFoundException(string id, Exception? innerException)
        : base($"User not found by ID '{id}'", innerException)
    {

    }
}
