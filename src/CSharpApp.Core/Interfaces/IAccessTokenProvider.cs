namespace CSharpApp.Core.Interfaces
{
    public interface IAccessTokenProvider
    {
        Task<LoginResponse> GetAccessTokenAsync(LoginRequest request, CancellationToken cancellationToken = default);
    }
}
