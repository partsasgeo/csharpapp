namespace CSharpApp.Tests.Auth.Commands;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    private static LoginCommand GenerateCommand(string? email, string? password)
    {
        return new LoginCommand(new LoginRequest { Email = email, Password = password });
    }

    [Fact]
    public void Validate_Succeeds_WhenEmailAndPasswordAreValid()
    {
        var result = _validator.TestValidate(GenerateCommand("user@example.com", "Password123"));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_Fails_WhenEmailIsEmpty(string? email)
    {
        var result = _validator.TestValidate(GenerateCommand(email, "Password123"));

        result.ShouldHaveValidationErrorFor("Email");
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("missing-at-sign.com")]
    [InlineData("user@")]
    public void Validate_Fails_WhenEmailIsNotAValidAddress(string email)
    {
        var result = _validator.TestValidate(GenerateCommand(email, "Password123"));

        result.ShouldHaveValidationErrorFor("Email")
            .WithErrorMessage("Email must be a valid email address.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_Fails_WhenPasswordIsEmpty(string? password)
    {
        var result = _validator.TestValidate(GenerateCommand("user@example.com", password));

        result.ShouldHaveValidationErrorFor("Password");
    }
}
