namespace CSharpApp.Tests.Products.Queries;

public class GetProductQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsProductFromService_ForRequestedId()
    {
        var product = new Product { Id = 7, Title = "Backpack" };
        var productsService = Substitute.For<IProductsService>();
        productsService.GetProduct(7, Arg.Any<CancellationToken>()).Returns(product);

        var handler = new GetProductQueryHandler(productsService);

        var result = await handler.Handle(new GetProductQuery(7), CancellationToken.None);

        Assert.Same(product, result);
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenServiceReturnsNull()
    {
        var productsService = Substitute.For<IProductsService>();
        productsService.GetProduct(404, Arg.Any<CancellationToken>()).Returns((Product?)null);

        var handler = new GetProductQueryHandler(productsService);

        var result = await handler.Handle(new GetProductQuery(404), CancellationToken.None);

        Assert.Null(result);
    }
}
