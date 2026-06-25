namespace CSharpApp.Tests.Categories.Commands;

public class CreateCategoryCommandHandlerTests
{
    [Fact]
    public async Task Handle_CreatesCategoryUsingService_ReturnsResult()
    {
        var request = new CreateCategoryRequest { Name = "Sports", Image = "https://example.com/sports.png" };
        var created = new Category { Id = 11, Name = "Sports", Image = "https://example.com/sports.png" };

        var categoriesService = Substitute.For<ICategoriesService>();
        categoriesService.CreateCategory(request, Arg.Any<CancellationToken>()).Returns(created);

        var commandHandler = new CreateCategoryCommandHandler(categoriesService);

        var result = await commandHandler.Handle(new CreateCategoryCommand(request), CancellationToken.None);

        Assert.Same(created, result);
    }
}
