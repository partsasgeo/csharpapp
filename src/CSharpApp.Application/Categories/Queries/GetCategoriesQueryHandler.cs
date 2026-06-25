namespace CSharpApp.Application.Categories.Queries;

public sealed class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, IReadOnlyCollection<Category>>
{
    private readonly ICategoriesService _categoriesService;

    public GetCategoriesQueryHandler(ICategoriesService CategoriesService)
    {
        _categoriesService = CategoriesService;
    }

    public Task<IReadOnlyCollection<Category>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        return _categoriesService.GetCategories(cancellationToken);
    }
}
