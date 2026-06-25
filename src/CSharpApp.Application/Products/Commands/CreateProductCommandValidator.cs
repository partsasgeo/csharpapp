using FluentValidation;

namespace CSharpApp.Application.Products.Commands
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Request.Price)
                .Must(x => x > 0).WithMessage($"{nameof(CreateProductRequest.Price)} should be > 0")
                .NotNull().WithMessage($"{nameof(CreateProductRequest.Price)} must not be empty.")
                .OverridePropertyName(nameof(CreateProductRequest.Price));

            RuleFor(x => x.Request.CategoryId)
                .Must(x => x > 0).WithMessage($"{nameof(CreateProductRequest.CategoryId)} must be an integer.") //external api reports back something about constraints - this is a more human friendly message (based on current state of the external api)
                .NotNull().WithMessage($"{nameof(CreateProductRequest.CategoryId)} must not be empty.")
                .OverridePropertyName(nameof(CreateProductRequest.CategoryId));

            RuleFor(x => x.Request.Title)
                .NotEmpty().WithMessage($"{nameof(CreateProductRequest.Title)} must not be empty.")
                .OverridePropertyName(nameof(CreateProductRequest.Title));

            RuleFor(x => x.Request.Description)
                .NotEmpty().WithMessage($"{nameof(CreateProductRequest.Description)} must not be empty.")
                .OverridePropertyName(nameof(CreateProductRequest.Description));

            RuleFor(x => x.Request.Images)
                .NotNull().WithMessage($"{nameof(CreateProductRequest.Images)} must not be empty.")
                .Must(x => x?.Any() == true).WithMessage($"{nameof(CreateProductRequest.Images)} must contain at least one image.")
                .OverridePropertyName(nameof(CreateProductRequest.Images));
        }
    }
}
