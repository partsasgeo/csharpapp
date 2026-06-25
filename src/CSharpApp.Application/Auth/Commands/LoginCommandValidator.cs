using FluentValidation;

namespace CSharpApp.Application.Auth.Commands;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Request.Email)
            .NotEmpty().WithMessage($"{nameof(LoginRequest.Email)} must not be empty.")
            .EmailAddress().WithMessage($"{nameof(LoginRequest.Email)} must be a valid email address.")
            .OverridePropertyName(nameof(LoginRequest.Email));

        RuleFor(x => x.Request.Password)
            .NotEmpty().WithMessage($"{nameof(LoginRequest.Password)} must not be empty.")
            .OverridePropertyName(nameof(LoginRequest.Password));
    }
}
