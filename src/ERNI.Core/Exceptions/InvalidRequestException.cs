
namespace ERNI.Core;

public sealed class InvalidRequestException : Exception
{
    public InvalidRequestException(string validationError)
        : this(validationError, null)
    {
    }

    public InvalidRequestException(string validationError, Exception? innerException)
        : base(validationError, innerException)
    {
    }
}
