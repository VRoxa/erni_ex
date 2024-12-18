using Autofac;
using FluentValidation;

namespace ERNI.Core;

internal class ValidatorProvider : IValidatorProvider
{
    private readonly ILifetimeScope _scope;

    public ValidatorProvider(ILifetimeScope scope)
    {
        _scope = scope;
    }

    public IValidator<T> Provide<T>(IValidatorProvider.ValidationType type)
    {
        return _scope.ResolveKeyed<IValidator<T>>(type);
    }
}

public interface IValidatorProvider
{
    public enum ValidationType
    {
        WithId,
        WithoutId
    }

    IValidator<T> Provide<T>(ValidationType type);
}
