namespace CSharpApp.Infrastructure.Configuration;

public static class DefaultConfiguration
{
    public static IServiceCollection AddDefaultConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RestApiSettings>(configuration!.GetSection(nameof(RestApiSettings)));
        services.Configure<HttpClientSettings>(configuration.GetSection(nameof(HttpClientSettings)));

        services.AddSingleton<IProductsService, ProductsService>();
        services.AddSingleton<ICategoriesService, CategoriesService>();
        services.AddSingleton<IAccessTokenProvider, AccessTokenProvider>();

        services.AddTransient<AuthorizationHandler>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetProductQuery).Assembly));

        return services;
    }
}