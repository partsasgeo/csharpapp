namespace CSharpApp.Tests.Products.Commands;

public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator = new();

    private static CreateProductCommand GenerateCommand(string? title, int? price, string? description, int? categoryId, List<string>? images)
    {
        return new CreateProductCommand(new CreateProductRequest
        {
            Title = title,
            Price = price,
            Description = description,
            CategoryId = categoryId,
            Images = images!
        });
    }

    [Fact]
    public void Validate_Succeeds_WhenAllFieldsAreValid()
    {
        var result = _validator.TestValidate(GenerateCommand("Shirt", 20, "A nice shirt", 1, ["https://example.com/shirt.png"]));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(null)]
    public void Validate_Fails_WhenPriceIsNotPositiveOrNull(int? price)
    {
        var result = _validator.TestValidate(GenerateCommand("Shirt", price, "A nice shirt", 1, ["https://example.com/shirt.png"]));

        result.ShouldHaveValidationErrorFor("Price");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(null)]
    public void Validate_Fails_WhenCategoryIdIsNotPositiveOrNull(int? categoryId)
    {
        var result = _validator.TestValidate(GenerateCommand("Shirt", 20, "A nice shirt", categoryId, ["https://example.com/shirt.png"]));

        result.ShouldHaveValidationErrorFor("CategoryId");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_Fails_WhenTitleIsEmpty(string? title)
    {
        var result = _validator.TestValidate(GenerateCommand(title, 20, "A nice shirt", 1, ["https://example.com/shirt.png"]));

        result.ShouldHaveValidationErrorFor("Title");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_Fails_WhenDescriptionIsEmpty(string? description)
    {
        var result = _validator.TestValidate(GenerateCommand("Shirt", 20, description, 1, ["https://example.com/shirt.png"]));

        result.ShouldHaveValidationErrorFor("Description");
    }

    [Fact]
    public void Validate_Fails_WhenImagesIsNull()
    {
        var result = _validator.TestValidate(GenerateCommand("Shirt", 20, "A nice shirt", 1, null));

        result.ShouldHaveValidationErrorFor("Images");
    }

    [Fact]
    public void Validate_Fails_WhenImagesIsEmpty()
    {
        var result = _validator.TestValidate(GenerateCommand("Shirt", 20, "A nice shirt", 1, []));

        result.ShouldHaveValidationErrorFor("Images");
    }
}
