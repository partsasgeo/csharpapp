namespace CSharpApp.Application.Products.Commands;

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Product>
{
    private readonly IProductsService _productsService;

    public CreateProductCommandHandler(IProductsService productsService)
    {
        _productsService = productsService;
    }

    public Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        return _productsService.CreateProduct(request.Request, cancellationToken);
    }
}
