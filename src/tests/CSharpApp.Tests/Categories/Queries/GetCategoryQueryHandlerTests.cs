namespace CSharpApp.Tests.Categories.Queries;

public class GetCategoryQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsCategoryFromService_ForRequestedId()
    {
        var category = new Category { Id = 7, Name = "Garden" };
        var categoriesService = Substitute.For<ICategoriesService>();
        categoriesService.GetCategory(7, Arg.Any<CancellationToken>()).Returns(category);

        var handler = new GetCategoryQueryHandler(categoriesService);

        var result = await handler.Handle(new GetCategoryQuery(7), CancellationToken.None);

        Assert.Same(category, result);
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenServiceReturnsNull()
    {
        var categoriesService = Substitute.For<ICategoriesService>();
        categoriesService.GetCategory(404, Arg.Any<CancellationToken>()).Returns((Category?)null);

        var handler = new GetCategoryQueryHandler(categoriesService);

        var result = await handler.Handle(new GetCategoryQuery(404), CancellationToken.None);

        Assert.Null(result);
    }
}
