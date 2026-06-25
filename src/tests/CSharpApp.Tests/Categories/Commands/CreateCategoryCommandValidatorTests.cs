namespace CSharpApp.Tests.Categories.Commands;

public class CreateCategoryCommandValidatorTests
{
    private readonly CreateCategoryCommandValidator _validator = new();

    private static CreateCategoryCommand GenerateCommand(string? name, string? image)
    {
        return new CreateCategoryCommand(new CreateCategoryRequest { Name = name, Image = image });
    }

    [Fact]
    public void Validate_Succeeds_WhenNameAndImageAreValid()
    {
        var result = _validator.TestValidate(GenerateCommand("Clothes", "https://example.com/clothes.png"));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_Fails_WhenNameIsEmpty(string? name)
    {
        var result = _validator.TestValidate(GenerateCommand(name, "https://example.com/clothes.png"));

        result.ShouldHaveValidationErrorFor("Name");
        result.ShouldNotHaveValidationErrorFor("Image");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_Fails_WhenImageIsEmpty(string? image)
    {
        var result = _validator.TestValidate(GenerateCommand("Clothes", image));

        result.ShouldHaveValidationErrorFor("Image");
    }

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("ftp:/missing-slash.com")]
    [InlineData("/relative/path.png")]
    public void Validate_Fails_WhenImageIsNotAnAbsoluteUrl(string image)
    {
        var result = _validator.TestValidate(GenerateCommand("Clothes", image));

        result.ShouldHaveValidationErrorFor("Image")
            .WithErrorMessage("Image must be a valid URL address.");
    }
}
