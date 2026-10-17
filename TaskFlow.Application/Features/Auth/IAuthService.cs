namespace TaskFlow.Application.Features.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

        Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

        Task<UserDto> GetCurrentUserAsync(CancellationToken cancellationToken = default);
    }
}