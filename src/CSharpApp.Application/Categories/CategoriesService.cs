using CSharpApp.Core.Constants;
using System.Net.Http.Json;

namespace CSharpApp.Application.Categories;

public class CategoriesService : ICategoriesService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly RestApiSettings _restApiSettings;
    private readonly ILogger<CategoriesService> _logger;

    public CategoriesService(IHttpClientFactory httpClientFactory,
        IOptions<RestApiSettings> restApiSettings,
        ILogger<CategoriesService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _restApiSettings = restApiSettings.Value;
        _logger = logger;
    }

    private string GetCategoriesPath()
    {
        var url = _restApiSettings.Categories?.TrimStart('/');
        if (string.IsNullOrWhiteSpace(url))
        {
            _logger.LogError("Method's Url is empty, entity {Entity}", nameof(_restApiSettings.Categories));
            throw new ArgumentException(nameof(_restApiSettings.Categories));
        }

        return url;
    }

    private async Task LogErrorsAndEnsureSuccessAsync(HttpResponseMessage response, string operation)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            _logger.LogError("{Operation} failed with {StatusCode}: {Body}", operation, response.StatusCode, errorBody);
        }
        response.EnsureSuccessStatusCode();
    }

    public async Task<IReadOnlyCollection<Category>> GetCategories()
    {
        var client = _httpClientFactory.CreateClient(Constants.EscuelaJsApiClient);
        var url = GetCategoriesPath();

        var response = await client.GetAsync(url);
        await LogErrorsAndEnsureSuccessAsync(response, nameof(GetCategories));
        var result = await response.Content.ReadFromJsonAsync<List<Category>>();
        return result.AsReadOnly();
    }

    public async Task<Category?> GetCategory(int id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        var client = _httpClientFactory.CreateClient(Constants.EscuelaJsApiClient);
        var url = GetCategoriesPath();

        var response = await client.GetAsync($"{url}/{id}");
        await LogErrorsAndEnsureSuccessAsync(response, nameof(GetCategory));
        var result = await response.Content.ReadFromJsonAsync<Category>();
        return result;
    }

    public async Task<Category> CreateCategory(CreateCategoryRequest request)
    {
        var client = _httpClientFactory.CreateClient(Constants.EscuelaJsApiClient);
        var url = GetCategoriesPath();

        var response = await client.PostAsJsonAsync(url, request);
        await LogErrorsAndEnsureSuccessAsync(response, nameof(CreateCategory));

        var result = await response.Content.ReadFromJsonAsync<Category>();
        return result;
    }
}
