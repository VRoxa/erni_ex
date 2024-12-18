using ERNI.Core.Validation;
using FluentValidation.TestHelper;

namespace ERNI.Core.Tests;

public class OrderDtoWithIdValidatorTests
{
    private static OrderDto ValidModel => new()
    {
        Id = Guid.NewGuid().ToString(),
        Amount = 10,
        UserId = Guid.NewGuid().ToString()
    };

    [Fact]
    public void OnValidate_IfValid_Passes()
    {
        //// Arrange
        var model = ValidModel;
        var sut = new OrderDtoWithIdValidator();

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
        var sut = new OrderDtoWithIdValidator();

        //// Act
        var result = sut.TestValidate(model);

        //// Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Malformatted")]
    public void OnValidate_IfUserIdIsInvalid_Fails(string userId)
    {
        //// Arrange
        var model = ValidModel with { UserId = userId };
        var sut = new OrderDtoWithIdValidator();

        //// Act
        var result = sut.TestValidate(model);

        //// Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-420)]
    public void OnValidate_IfAmountIsInvalid_Fails(int amount)
    {
        //// Arrange
        var model = ValidModel with { Amount = amount };
        var sut = new OrderDtoWithIdValidator();

        //// Act
        var result = sut.TestValidate(model);

        //// Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }
}
