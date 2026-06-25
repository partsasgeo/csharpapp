namespace CSharpApp.Tests.Products.Queries;

public class GetProductsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsProductsFromService()
    {
        var products = new List<Product>
        {
            new() { Id = 1, Title = "Shirt" }
        }.AsReadOnly();

        var productsService = Substitute.For<IProductsService>();
        productsService.GetProducts(Arg.Any<CancellationToken>()).Returns(products);

        var handler = new GetProductsQueryHandler(productsService);

        var result = await handler.Handle(new GetProductsQuery(), CancellationToken.None);

        Assert.Same(products, result);
    }
}
