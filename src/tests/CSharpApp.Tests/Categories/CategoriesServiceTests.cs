using System.Net;
using System.Net.Http.Json;
using CSharpApp.Tests.TestHelpers;

namespace CSharpApp.Tests.Categories;

public class CategoriesServiceTests
{
    private static CategoriesService CreateSubstitutedService(FakeHttpMessageHandler handler, string? categoriesPath = "categories")
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://example.com/") };
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory.CreateClient(Constants.EscuelaJsApiAuthenticatedClient).Returns(httpClient);

        var settings = Options.Create(new RestApiSettings { Categories = categoriesPath });
        var logger = Substitute.For<ILogger<CategoriesService>>();

        return new CategoriesService(httpClientFactory, settings, logger);
    }

    private static FakeHttpMessageHandler SuccessHandler<T>(T payload) =>
        new(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(payload)
        });

    [Fact]
    public async Task GetCategories_ReturnsCategories_OnSuccessfulResponse()
    {
        var categories = new List<Category>
        {
            new() { Id = 1, Name = "Clothes", Image = "https://example.com/clothes.png" },
            new() { Id = 2, Name = "Electronics", Image = "https://example.com/electronics.png" }
        };
        var handler = SuccessHandler(categories);
        var service = CreateSubstitutedService(handler);

        var result = await service.GetCategories();

        Assert.Equal(2, result.Count);
        Assert.Equal("Clothes", result.First().Name);
        Assert.Equal(HttpMethod.Get, handler.RequestMessage?.Method);
        Assert.Equal("https://example.com/categories", handler.RequestMessage?.RequestUri?.OriginalString);
    }

    [Fact]
    public async Task GetCategories_ThrowsHttpRequestException_OnUnsuccessfulResponse()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("Internal Error")
        });
        var service = CreateSubstitutedService(handler);

        await Assert.ThrowsAsync<HttpRequestException>(() => service.GetCategories());
    }

    [Fact]
    public async Task GetCategories_ThrowsArgumentException_WhenCategoriesPathIsNotSet()
    {
        var handler = SuccessHandler(new List<Category>());
        var service = CreateSubstitutedService(handler, categoriesPath: null);

        await Assert.ThrowsAsync<ArgumentException>(() => service.GetCategories());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetCategory_ThrowsArgumentOutOfRangeException_WhenIdIsNotPositive(int id)
    {
        var handler = SuccessHandler(new Category());
        var service = CreateSubstitutedService(handler);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.GetCategory(id));
    }

    [Fact]
    public async Task GetCategory_RequestsCategoryByIdAndReturnsIt()
    {
        var category = new Category { Id = 5, Name = "Books", Image = "https://example.com/books.png" };
        var handler = SuccessHandler(category);
        var service = CreateSubstitutedService(handler);

        var result = await service.GetCategory(5);

        Assert.NotNull(result);
        Assert.Equal("Books", result!.Name);
        Assert.Equal("https://example.com/categories/5", handler.RequestMessage?.RequestUri?.OriginalString);
    }

    [Fact]
    public async Task CreateCategory_PostsRequestAndReturnsCreatedCategory()
    {
        var created = new Category { Id = 9, Name = "Toys", Image = "https://example.com/toys.png" };
        var handler = SuccessHandler(created);
        var service = CreateSubstitutedService(handler);
        var request = new CreateCategoryRequest { Name = "Toys", Image = "https://example.com/toys.png" };

        var result = await service.CreateCategory(request);

        Assert.Equal(9, result.Id);
        Assert.Equal(HttpMethod.Post, handler.RequestMessage?.Method);
        Assert.Equal("https://example.com/categories", handler.RequestMessage?.RequestUri?.OriginalString);
    }
}
