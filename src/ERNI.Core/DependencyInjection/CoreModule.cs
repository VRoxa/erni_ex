using Autofac;
using ERNI.Core.Validation;
using FluentValidation;

namespace ERNI.Core.DependencyInjection;

public sealed class CoreModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<UsersService>().As<IUsersService>();
        builder.RegisterType<OrdersService>().As<IOrdersService>();

        builder.RegisterDecorator<UsersValidationService, IUsersService>();
        builder.RegisterDecorator<OrdersValidationService, IOrdersService>();

        builder.RegisterType<Clock>().As<IClock>();

        builder.RegisterType<ValidatorProvider>().As<IValidatorProvider>();
        builder.RegisterType<OrderDtoValidator>().Keyed<IValidator<OrderDto>>(IValidatorProvider.ValidationType.WithoutId);
        builder.RegisterType<OrderDtoWithIdValidator>().Keyed<IValidator<OrderDto>>(IValidatorProvider.ValidationType.WithId);
        builder.RegisterType<UserDtoValidator>().Keyed<IValidator<UserDto>>(IValidatorProvider.ValidationType.WithoutId);
        builder.RegisterType<UserDtoWithIdValidator>().Keyed<IValidator<UserDto>>(IValidatorProvider.ValidationType.WithId);
    }
}
