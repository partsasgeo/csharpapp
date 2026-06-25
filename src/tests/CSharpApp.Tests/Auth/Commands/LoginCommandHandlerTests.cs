namespace CSharpApp.Tests.Auth.Commands;

public class LoginCommandHandlerTests
{
    [Fact]
    public async Task Handle_LogsInUsingAccessTokenProvider_ReturnsResult()
    {
        var request = new LoginRequest { Email = "user@example.com", Password = "Password123" };
        var response = new LoginResponse { AccessToken = "access-token", RefreshToken = "refresh-token" };

        var accessTokenProvider = Substitute.For<IAccessTokenProvider>();
        accessTokenProvider.GetAccessTokenAsync(request, Arg.Any<CancellationToken>()).Returns(response);

        var commandHandler = new LoginCommandHandler(accessTokenProvider);

        var result = await commandHandler.Handle(new LoginCommand(request), CancellationToken.None);

        Assert.Same(response, result);
    }
}
