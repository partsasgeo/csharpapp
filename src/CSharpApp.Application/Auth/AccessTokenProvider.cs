using CSharpApp.Core.Constants;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace CSharpApp.Application.Auth
{
    public class AccessTokenProvider : IAccessTokenProvider
    {
        private sealed class JwtPayload
        {
            [JsonPropertyName("exp")]
            public long Exp { get; set; }
        }

        private const int BeforeExpiringSeconds = 30;
        private static readonly TimeSpan DefaultRefreshTimeSpan = TimeSpan.FromMinutes(15);

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly RestApiSettings _restApiSettings;
        private readonly ILogger<AccessTokenProvider> _logger;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        private LoginResponse? _cachedToken;
        private DateTimeOffset _expiresAtUtc = DateTimeOffset.MinValue;

        public AccessTokenProvider(IHttpClientFactory httpClientFactory,
            IOptions<RestApiSettings> apiSettings,
            ILogger<AccessTokenProvider> logger)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _restApiSettings = apiSettings.Value;
            _logger = logger;
        }

        private string GetAuthPath()
        {
            var url = _restApiSettings.Auth?.TrimStart('/');
            if (string.IsNullOrWhiteSpace(url))
            {
                _logger.LogError("Method's Url is empty, entity {Entity}", nameof(_restApiSettings.Auth));
                throw new ArgumentException(nameof(_restApiSettings.Auth));
            }

            return url;
        }

        private async Task LogErrorsAndEnsureSuccessAsync(HttpResponseMessage response, string operation, CancellationToken cancellationToken)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("{Operations} failed with {StatusCode}: {Body}", operation, response.StatusCode, errorBody);
            }
            response.EnsureSuccessStatusCode();
        }

        public async Task<LoginResponse> GetAccessTokenAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            if (_cachedToken is not null && DateTimeOffset.UtcNow < _expiresAtUtc)
            {
                return _cachedToken;
            }

            //use semaphoreSlim to ensure that the token generation is performed only once
            //regardless of concurrent calls here - in the case of token already expiring
            //check for expiration should exist in the WaitAsync so that a new request actually sees that token is freshly generated
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                if (_cachedToken is not null && DateTimeOffset.UtcNow < _expiresAtUtc)
                {
                    return _cachedToken;
                }

                var client = _httpClientFactory.CreateClient(Constants.EscuelaJsApiClient);
                var url = GetAuthPath();

                var response = await client.PostAsJsonAsync(url, request, cancellationToken);
                await LogErrorsAndEnsureSuccessAsync(response, nameof(GetAccessTokenAsync), cancellationToken);

                var result = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken);
                if (string.IsNullOrWhiteSpace(result?.AccessToken))
                {
                    throw new InvalidOperationException("Login response did not contain an access token.");
                }

                _expiresAtUtc = GenerateTokenExpiryDateTime(result.AccessToken);
                _cachedToken = result;

                return result;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private DateTimeOffset GenerateTokenExpiryDateTime(string accessToken)
        {
            try
            {
                //JWT Token includes a BASE64 encoded object that after decoding
                //results in a response of type {"alg":"ENCRYPTIONALG","typ":"JWT"}{"sub":int,"iat":unixTimestamp,"exp":unixTimestamp} and some extra sections/segments
                //Decode the 2nd segment to get the exp (expiry date)

                var segments = accessToken.Split('.');
                if (segments.Length < 2)
                {
                    throw new FormatException("Token fetched is not a JWT token.");
                }

                var payloadJson = Base64UrlDecode(segments[1]);
                var payload = JsonSerializer.Deserialize<JwtPayload>(payloadJson);
                if (payload is null || payload.Exp <= 0)
                {
                    throw new FormatException("Token payload did not contain an 'exp' claim.");
                }

                //return back the dateTime of expiring minus some seconds before
                //(actually cleanup token from cache and refetch a new one right before the previous expires)
                return DateTimeOffset.FromUnixTimeSeconds(payload.Exp) - TimeSpan.FromSeconds(BeforeExpiringSeconds);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not decode the JWT 'exp' claim, setting default timespan to refresh token ({DefaultSpan} Minutes).", DefaultRefreshTimeSpan);
                return DateTimeOffset.UtcNow + DefaultRefreshTimeSpan - TimeSpan.FromSeconds(BeforeExpiringSeconds);
            }
        }

        private static byte[] Base64UrlDecode(string segment)
        {
            var base64 = segment.Replace('-', '+').Replace('_', '/');
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            return Convert.FromBase64String(base64);
        }
    }
}
