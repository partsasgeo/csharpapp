namespace CSharpApp.Application.Auth.Commands;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IAccessTokenProvider _accessTokenProvider;

    public LoginCommandHandler(IAccessTokenProvider accessTokenProvider)
    {
        _accessTokenProvider = accessTokenProvider;
    }

    public Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return _accessTokenProvider.GetAccessTokenAsync(request.Request, cancellationToken);
    }
}
