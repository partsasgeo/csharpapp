namespace CSharpApp.Application.Categories.Commands;

public sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Category>
{
    private readonly ICategoriesService _categoriesService;

    public CreateCategoryCommandHandler(ICategoriesService categoriesService)
    {
        _categoriesService = categoriesService;
    }

    public Task<Category> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        return _categoriesService.CreateCategory(request.Request, cancellationToken);
    }
}
