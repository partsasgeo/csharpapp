using CSharpApp.Core.Constants;

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

    public async Task<IReadOnlyCollection<Product>> GetProducts()
    {
        try
        {
            var client = _httpClientFactory.CreateClient(Constants.EscuelaJsApiClient);
            var url = _restApiSettings.Products?.TrimStart('/');
            if (string.IsNullOrWhiteSpace(url))
            {
                _logger.LogError("Method's Url is empty, entity {Entity}", nameof(_restApiSettings.Products));
                throw new ArgumentException(nameof(_restApiSettings.Products));
            }
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var res = JsonSerializer.Deserialize<List<Product>>(content);

            return res.AsReadOnly();
        }
        catch (Exception)
        {
            throw;
        }
    }
}