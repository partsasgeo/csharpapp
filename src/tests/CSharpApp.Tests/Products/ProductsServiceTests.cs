using System.Net;
using System.Net.Http.Json;
using CSharpApp.Tests.TestHelpers;

namespace CSharpApp.Tests.Products;

public class ProductsServiceTests
{
    private static ProductsService CreateSubstitutedService(FakeHttpMessageHandler handler, string? productsPath = "products")
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://example.com/") };
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory.CreateClient(Constants.EscuelaJsApiAuthenticatedClient).Returns(httpClient);

        var settings = Options.Create(new RestApiSettings { Products = productsPath });
        var logger = Substitute.For<ILogger<ProductsService>>();

        return new ProductsService(httpClientFactory, settings, logger);
    }

    private static FakeHttpMessageHandler SuccessHandler<T>(T payload) =>
        new(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(payload)
        });

    [Fact]
    public async Task GetProducts_ReturnsProducts_OnSuccessfulResponse()
    {
        var products = new List<Product>
        {
            new() { Id = 1, Title = "Shirt", Price = 20 },
            new() { Id = 2, Title = "Shoes", Price = 50 }
        };
        var handler = SuccessHandler(products);
        var service = CreateSubstitutedService(handler);

        var result = await service.GetProducts();

        Assert.Equal(2, result.Count);
        Assert.Equal("Shirt", result.First().Title);
        Assert.Equal(HttpMethod.Get, handler.RequestMessage?.Method);
        Assert.Equal("https://example.com/products", handler.RequestMessage?.RequestUri?.OriginalString);
    }

    [Fact]
    public async Task GetProducts_ThrowsHttpRequestException_OnUnsuccessfulResponse()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("Internal Error")
        });
        var service = CreateSubstitutedService(handler);

        await Assert.ThrowsAsync<HttpRequestException>(() => service.GetProducts());
    }

    [Fact]
    public async Task GetProducts_ThrowsArgumentException_WhenProductsPathIsNotSet()
    {
        var handler = SuccessHandler(new List<Product>());
        var sut = CreateSubstitutedService(handler, productsPath: null);

        await Assert.ThrowsAsync<ArgumentException>(() => sut.GetProducts());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetProduct_ThrowsArgumentOutOfRangeException_WhenIdIsNotPositive(int id)
    {
        var handler = SuccessHandler(new Product());
        var service = CreateSubstitutedService(handler);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.GetProduct(id));
    }

    [Fact]
    public async Task GetProduct_RequestsProductByIdAndReturnsIt()
    {
        var product = new Product { Id = 5, Title = "Jacket", Price = 80 };
        var handler = SuccessHandler(product);
        var service = CreateSubstitutedService(handler);

        var result = await service.GetProduct(5);

        Assert.NotNull(result);
        Assert.Equal("Jacket", result!.Title);
        Assert.Equal("https://example.com/products/5", handler.RequestMessage?.RequestUri?.OriginalString);
    }

    [Fact]
    public async Task CreateProduct_PostsRequestAndReturnsCreatedProduct()
    {
        var created = new Product { Id = 9, Title = "Hat", Price = 15 };
        var handler = SuccessHandler(created);
        var service = CreateSubstitutedService(handler);
        var request = new CreateProductRequest
        {
            Title = "Hat",
            Price = 15,
            Description = "A hat",
            CategoryId = 1,
            Images = ["https://example.com/hat.png"]
        };

        var result = await service.CreateProduct(request);

        Assert.Equal(9, result.Id);
        Assert.Equal(HttpMethod.Post, handler.RequestMessage?.Method);
        Assert.Equal("https://example.com/products", handler.RequestMessage?.RequestUri?.OriginalString);
    }
}
