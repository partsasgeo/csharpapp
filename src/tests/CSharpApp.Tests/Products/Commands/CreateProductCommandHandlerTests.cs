namespace CSharpApp.Tests.Products.Commands;

public class CreateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_CreatesProductUsingService_ReturnsResult()
    {
        var request = new CreateProductRequest
        {
            Title = "Sneakers",
            Price = 60,
            Description = "Running shoes",
            CategoryId = 2,
            Images = ["https://example.com/sneakers.png"]
        };
        var created = new Product { Id = 11, Title = "Sneakers", Price = 60 };

        var productsService = Substitute.For<IProductsService>();
        productsService.CreateProduct(request, Arg.Any<CancellationToken>()).Returns(created);

        var commandHandler = new CreateProductCommandHandler(productsService);

        var result = await commandHandler.Handle(new CreateProductCommand(request), CancellationToken.None);

        Assert.Same(created, result);
    }
}
