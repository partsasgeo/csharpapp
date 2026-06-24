namespace CSharpApp.Core.Interfaces;

public interface IProductsService
{
    Task<IReadOnlyCollection<Product>> GetProducts(CancellationToken cancellationToken = default);
    Task<Product?> GetProduct(int id, CancellationToken cancellationToken = default);
    Task<Product> CreateProduct(CreateProductRequest request, CancellationToken cancellationToken = default);
}
