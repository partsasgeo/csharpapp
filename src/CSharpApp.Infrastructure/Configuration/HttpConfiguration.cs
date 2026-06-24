using CSharpApp.Core.Constants;
using Polly;
using Polly.Extensions.Http;

namespace CSharpApp.Infrastructure.Configuration;

public static class HttpConfiguration
{
    public static IServiceCollection AddHttpConfiguration(this IServiceCollection services, RestApiSettings apiSettings, HttpClientSettings clientSettings)
    {
        if (string.IsNullOrWhiteSpace(apiSettings.BaseUrl))
        {
            throw new ArgumentException($"{nameof(RestApiSettings)}.{nameof(RestApiSettings.BaseUrl)} cannot be empty.");
        }

        services
            .AddHttpClient(Constants.EscuelaJsApiClient, clientConfig =>
            {
                clientConfig.BaseAddress = new Uri(apiSettings.BaseUrl);
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(clientSettings.LifeTime))
            .AddPolicyHandler(GetAsyncPolicy(clientSettings));

        //HttpClient that passes the auth JWT Token as bearer to all requests
        services
            .AddHttpClient(Constants.EscuelaJsApiAuthenticatedClient, clientConfig =>
            {
                clientConfig.BaseAddress = new Uri(apiSettings.BaseUrl);
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(clientSettings.LifeTime))
            .AddPolicyHandler(GetAsyncPolicy(clientSettings))
            .AddHttpMessageHandler<AuthorizationHandler>();

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetAsyncPolicy(HttpClientSettings settings)
    {
        return HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(settings.RetryCount, retryAttempt => TimeSpan.FromMilliseconds(retryAttempt * settings.SleepDuration));
    }
}