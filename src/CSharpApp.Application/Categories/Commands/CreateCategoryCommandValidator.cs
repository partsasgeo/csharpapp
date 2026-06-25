using FluentValidation;

namespace CSharpApp.Application.Categories.Commands;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Request.Name)
            .NotEmpty().WithMessage($"{nameof(CreateCategoryRequest.Name)} must not be empty.")
            .OverridePropertyName(nameof(CreateCategoryRequest.Name));

        RuleFor(x => x.Request.Image)
            .NotEmpty().WithMessage($"{nameof(CreateCategoryRequest.Image)} must not be empty.")
            .Must(x => Uri.TryCreate(x, UriKind.Absolute, out _)).WithMessage($"{nameof(CreateCategoryRequest.Image)} must be a valid URL address.")
            .OverridePropertyName(nameof(CreateCategoryRequest.Image));
    }
}
