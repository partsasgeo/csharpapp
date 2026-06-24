using CSharpApp.Api.ExceptionHandling;
using CSharpApp.Core.Dtos;
using CSharpApp.Core.Settings;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Logging.ClearProviders().AddSerilog(logger);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDefaultConfiguration(builder.Configuration);
builder.Services.AddHttpConfiguration(
    builder.Configuration.GetSection(nameof(RestApiSettings)).Get<RestApiSettings>()!,
    builder.Configuration.GetSection(nameof(HttpClientSettings)).Get<HttpClientSettings>()!
    );
builder.Services.AddExceptionHandler<ExternalApiExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddApiVersioning();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
//app.UseHttpsRedirection();

var versionedEndpointRouteBuilder = app.NewVersionedApi();

versionedEndpointRouteBuilder.MapGet("api/v{version:apiVersion}/getproducts", async (IProductsService productsService, CancellationToken cancellationToken) =>
    {
        var products = await productsService.GetProducts(cancellationToken);
        //In case of exception the following check is irrelevant,
        //however if it somehow returns null then handle it
        return products != null ? Results.Ok(products) : Results.BadRequest();
    })
    .WithName("GetProducts")
    .HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapGet("api/v{version:apiVersion}/getproduct/{id:int}", async (int id, IProductsService productsService, CancellationToken cancellationToken) =>
    {
        var product = await productsService.GetProduct(id, cancellationToken);
        return product != null ? Results.Ok(product) : Results.NotFound(product);
    })
    .WithName("GetProduct")
    .HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapPost("api/v{version:apiVersion}/createproduct", async (CreateProductRequest request, IProductsService productsService, CancellationToken cancellationToken) =>
    {
        var product = await productsService.CreateProduct(request, cancellationToken);
        return product != null ? Results.Created($"api/v1/getproduct/{product.Id}", product) : Results.BadRequest();
    })
    .WithName("CreateProduct")
    .HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapGet("api/v{version:apiVersion}/getcategories", async (ICategoriesService categoriesService, CancellationToken cancellationToken) =>
    {
        var categories = await categoriesService.GetCategories(cancellationToken);
        return categories != null ? Results.Ok(categories) : Results.BadRequest();
    })
    .WithName("GetCategories")
    .HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapGet("api/v{version:apiVersion}/getcategory/{id:int}", async (int id, ICategoriesService categoriesService, CancellationToken cancellationToken) =>
    {
        var category = await categoriesService.GetCategory(id, cancellationToken);
        return category != null ? Results.Ok(category) : Results.NotFound(category);
    })
    .WithName("GetCategory")
    .HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapPost("api/v{version:apiVersion}/createcategory", async (CreateCategoryRequest request, ICategoriesService categoriesService, CancellationToken cancellationToken) =>
    {
        var category = await categoriesService.CreateCategory(request, cancellationToken);
        return category != null ? Results.Created($"api/v1/getcategory/{category.Id}", category) : Results.BadRequest();
    })
    .WithName("CreateCategory")
    .HasApiVersion(1.0);

app.Run();
