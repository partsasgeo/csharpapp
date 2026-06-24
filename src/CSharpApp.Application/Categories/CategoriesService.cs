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

    private async Task LogErrorsAndEnsureSuccessAsync(HttpResponseMessage response, string operation, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("{Operation} failed with {StatusCode}: {Body}", operation, response.StatusCode, errorBody);
        }
        response.EnsureSuccessStatusCode();
    }

    public async Task<IReadOnlyCollection<Category>> GetCategories(CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient(Constants.EscuelaJsApiClient);
        var url = GetCategoriesPath();

        var response = await client.GetAsync(url, cancellationToken);
        await LogErrorsAndEnsureSuccessAsync(response, nameof(GetCategories), cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<List<Category>>(cancellationToken);
        return result.AsReadOnly();
    }

    public async Task<Category?> GetCategory(int id, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        var client = _httpClientFactory.CreateClient(Constants.EscuelaJsApiClient);
        var url = GetCategoriesPath();

        var response = await client.GetAsync($"{url}/{id}", cancellationToken);
        await LogErrorsAndEnsureSuccessAsync(response, nameof(GetCategory), cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<Category>(cancellationToken);
        return result;
    }

    public async Task<Category> CreateCategory(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient(Constants.EscuelaJsApiClient);
        var url = GetCategoriesPath();

        var response = await client.PostAsJsonAsync(url, request, cancellationToken);
        await LogErrorsAndEnsureSuccessAsync(response, nameof(CreateCategory), cancellationToken);

        var result = await response.Content.ReadFromJsonAsync<Category>(cancellationToken);
        return result;
    }
}
