namespace CSharpApp.Tests.Categories.Queries;

public class GetCategoriesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsCategoriesFromService()
    {
        var categories = new List<Category>
        {
            new() { Id = 1, Name = "Clothes" }
        }.AsReadOnly();

        var categoriesService = Substitute.For<ICategoriesService>();
        categoriesService.GetCategories(Arg.Any<CancellationToken>()).Returns(categories);

        var handler = new GetCategoriesQueryHandler(categoriesService);

        var result = await handler.Handle(new GetCategoriesQuery(), CancellationToken.None);

        Assert.Same(categories, result);
    }
}
