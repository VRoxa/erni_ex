using FluentValidation;

namespace ERNI.Core.Validation;

public class OrderDtoValidator : AbstractValidator<OrderDto>
{
    public OrderDtoValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.UserId)
            .MustBeGuid()
            .WithMessage("Malformatted GUID in User ID");
    }
}

public class OrderDtoWithIdValidator : OrderDtoValidator
{
    public OrderDtoWithIdValidator()
        : base()
    {
        RuleFor(x => x.Id)
            .MustBeGuid()
            .WithMessage("Malformatted GUID in Order ID");
    }
}
