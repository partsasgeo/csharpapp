namespace CSharpApp.Application.Products.Queries;

public sealed class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IReadOnlyCollection<Product>>
{
    private readonly IProductsService _productsService;

    public GetProductsQueryHandler(IProductsService productsService)
    {
        _productsService = productsService;
    }

    public Task<IReadOnlyCollection<Product>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        return _productsService.GetProducts(cancellationToken);
    }
}
