namespace CSharpApp.Application.Products.Queries;

public sealed class GetProductQueryHandler : IRequestHandler<GetProductQuery, Product?>
{
    private readonly IProductsService _productsService;

    public GetProductQueryHandler(IProductsService productsService)
    {
        _productsService = productsService;
    }

    public Task<Product?> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        return _productsService.GetProduct(request.Id, cancellationToken);
    }
}
