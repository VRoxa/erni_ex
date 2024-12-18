using FluentValidation;

namespace ERNI.Core.Validation;

public class UserDtoValidator : AbstractValidator<UserDto>
{
    public UserDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Email).EmailAddress().When(x => x.Email is not null);
    }
}

public class UserDtoWithIdValidator : UserDtoValidator
{
    public UserDtoWithIdValidator()
    {
        RuleFor(x => x.Id)
            .MustBeGuid()
            .WithMessage("Malformatted GUID in User ID");
    }
}
