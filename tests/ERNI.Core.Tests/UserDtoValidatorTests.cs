using ERNI.Core.Validation;
using FluentValidation.TestHelper;

namespace ERNI.Core.Tests;

public class UserDtoValidatorTests
{
    private static UserDto ValidModel => new()
    {
        Name = "John Doe",
        Email = "john.doe@erni.com"
    };

    [Fact]
    public void OnValidate_IfValid_Passes()
    {
        //// Arrange
        var model = ValidModel;
        var sut = new UserDtoValidator();

        //// Act
        var result = sut.TestValidate(model);

        //// Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void OnValidate_IfNameIsInvalid_Fails(string name)
    {
        //// Arrange
        var model = ValidModel with { Name = name };
        var sut = new UserDtoValidator();

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
        var sut = new UserDtoValidator();

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
        var sut = new UserDtoValidator();

        //// Act
        var result = sut.TestValidate(model);

        //// Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
