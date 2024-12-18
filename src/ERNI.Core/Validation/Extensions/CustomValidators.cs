using FluentValidation;

namespace ERNI.Core;

public static class CustomValidators
{
    public static IRuleBuilderOptions<T, string?> MustBeGuid<T>(this IRuleBuilder<T, string?> builder)
    {
        return builder.Must(x => Guid.TryParse(x, out _));
    }
}
