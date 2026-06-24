using System.Net.Http.Headers;

namespace CSharpApp.Application.Auth;

public class AuthorizationHandler : DelegatingHandler
{
    private readonly IAccessTokenProvider _accessTokenProvider;
    private readonly RestApiSettings _restApiSettings;

    public AuthorizationHandler(IAccessTokenProvider accessTokenProvider, IOptions<RestApiSettings> restApiSettings)
    {
        _accessTokenProvider = accessTokenProvider;
        _restApiSettings = restApiSettings.Value;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        //fetch or refresh token for each request
        //only one request will issue the token, all other will get the cached one
        var loginRequest = new LoginRequest { Email = _restApiSettings.Username, Password = _restApiSettings.Password };
        var token = await _accessTokenProvider.GetAccessTokenAsync(loginRequest, cancellationToken);

        if (!string.IsNullOrWhiteSpace(token.AccessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
