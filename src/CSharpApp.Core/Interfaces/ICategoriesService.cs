namespace CSharpApp.Core.Interfaces;

public interface ICategoriesService
{
    Task<IReadOnlyCollection<Category>> GetCategories(CancellationToken cancellationToken = default);
    Task<Category?> GetCategory(int id, CancellationToken cancellationToken = default);
    Task<Category> CreateCategory(CreateCategoryRequest request, CancellationToken cancellationToken = default);
}
