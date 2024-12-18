namespace ERNI.Core;

internal sealed class Clock : IClock
{
    public DateTime Now => DateTime.Now;
}
