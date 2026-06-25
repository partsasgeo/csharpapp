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

    private HttpClient CreateClient()
    {
        return _httpClientFactory.CreateClient(Constants.EscuelaJsApiAuthenticatedClient);
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
    private async Task LogErrorsAndEnsureSuccessAsync(HttpResponseMessage response, string operation, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("{Operation} failed with {StatusCode}: {Body}", operation, response.StatusCode, errorBody);
        }
        response.EnsureSuccessStatusCode();
    }

    public async Task<IReadOnlyCollection<Product>> GetProducts(CancellationToken cancellationToken = default)
    {
        try
        {
            var client = CreateClient();
            var url = GetProductsPath();

            var response = await client.GetAsync(url, cancellationToken);
            await LogErrorsAndEnsureSuccessAsync(response, nameof(GetProducts), cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<List<Product>>(cancellationToken);
            return result.AsReadOnly();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Product?> GetProduct(int id, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        var client = CreateClient();
        var url = GetProductsPath();

        var response = await client.GetAsync($"{url}/{id}", cancellationToken);
        await LogErrorsAndEnsureSuccessAsync(response, nameof(GetProduct), cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<Product>(cancellationToken);
        return result;
    }

    public async Task<Product> CreateProduct(CreateProductRequest request, CancellationToken cancellationToken = default)
    {   
        var client = CreateClient();
        var url = GetProductsPath();

        var response = await client.PostAsJsonAsync(url, request, cancellationToken);
        await LogErrorsAndEnsureSuccessAsync(response, nameof(CreateProduct), cancellationToken);

        var result = await response.Content.ReadFromJsonAsync<Product>(cancellationToken);
        return result;
    }
}
