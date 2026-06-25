namespace CSharpApp.Application.Categories.Queries;

public sealed class GetCategoryQueryHandler : IRequestHandler<GetCategoryQuery, Category?>
{
    private readonly ICategoriesService _categoriesService;

    public GetCategoryQueryHandler(ICategoriesService categoriesService)
    {
        _categoriesService = categoriesService;
    }

    public Task<Category?> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
    {
        return _categoriesService.GetCategory(request.Id, cancellationToken);
    }
}
