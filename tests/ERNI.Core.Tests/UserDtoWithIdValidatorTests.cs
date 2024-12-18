using ERNI.Core.Validation;
using FluentValidation.TestHelper;

namespace ERNI.Core.Tests;

public class UserDtoWithIdValidatorTests
{
    private static UserDto ValidModel => new()
    {
        Id = Guid.NewGuid().ToString(),
        Name = "John Doe",
        Email = "john.doe@erni.com"
    };

    [Fact]
    public void OnValidate_IfValid_Passes()
    {
        //// Arrange
        var model = ValidModel;
        var sut = new UserDtoWithIdValidator();

        //// Act
        var result = sut.TestValidate(model);

        //// Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Malformatted")]
    public void OnValidate_IfIdIsInvalid_Fails(string userId)
    {
        //// Arrange
        var model = ValidModel with { Id = userId };
        var sut = new UserDtoWithIdValidator();

        //// Act
        var result = sut.TestValidate(model);

        //// Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void OnValidate_IfNameIsInvalid_Fails(string name)
    {
        //// Arrange
        var model = ValidModel with { Name = name };
        var sut = new UserDtoWithIdValidator();

        //// Act
        var result = sut.TestValidate(model);

        //// Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void OnValidate_IfEmailIsInvalid_Fails()
    {
        //// Arrange
        var model = ValidModel with { Email = "Malformatted" };
        var sut = new UserDtoWithIdValidator();

        //// Act
        var result = sut.TestValidate(model);

        //// Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void OnValidate_IfEmailIsNull_Passes()
    {
        //// Arrange
        var model = ValidModel with { Email = null };
        var sut = new UserDtoWithIdValidator();

        //// Act
        var result = sut.TestValidate(model);

        //// Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
