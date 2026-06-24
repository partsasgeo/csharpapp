using CSharpApp.Core.Constants;
using System.Net.Http.Json;

namespace CSharpApp.Application.Products;

public class ProductsService : IProductsService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly RestApiSettings _restApiSettings;
    private readonly ILogger<ProductsService> _logger;

    public ProductsService(IHttpClientFactory httpClientFactory,
        IOptions<RestApiSettings> restApiSettings,
        ILogger<ProductsService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _restApiSettings = restApiSettings.Value;
        _logger = logger;
    }

    private string GetProductsPath()
    {
        var url = _restApiSettings.Products?.TrimStart('/');
        if (string.IsNullOrWhiteSpace(url))
        {
            _logger.LogError("Method's Url is empty, entity {Entity}", nameof(_restApiSettings.Products));
            throw new ArgumentException(nameof(_restApiSettings.Products));
        }

        return url;
    }
    private async Task LogErrorsAndEnsureSuccessAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            _logger.LogError("CreateProduct failed with {StatusCode}: {Body}", response.StatusCode, errorBody);
        }
        response.EnsureSuccessStatusCode();
    }

    public async Task<IReadOnlyCollection<Product>> GetProducts()
    {
        try
        {
            var client = _httpClientFactory.CreateClient(Constants.EscuelaJsApiClient);
            var url = GetProductsPath();

            var response = await client.GetAsync(url);
            await LogErrorsAndEnsureSuccessAsync(response);
            var result = await response.Content.ReadFromJsonAsync<List<Product>>();
            return result.AsReadOnly();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Product?> GetProduct(int id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        var client = _httpClientFactory.CreateClient(Constants.EscuelaJsApiClient);
        var url = GetProductsPath();

        var response = await client.GetAsync($"{url}/{id}");
        await LogErrorsAndEnsureSuccessAsync(response);
        var result = await response.Content.ReadFromJsonAsync<Product>();
        return result;
    }

    public async Task<Product> CreateProduct(CreateProductRequest request)
    {
        var client = _httpClientFactory.CreateClient(Constants.EscuelaJsApiClient);
        var url = GetProductsPath();

        var response = await client.PostAsJsonAsync(url, request);
        await LogErrorsAndEnsureSuccessAsync(response);

        var result = await response.Content.ReadFromJsonAsync<Product>();
        return result;
    }
}