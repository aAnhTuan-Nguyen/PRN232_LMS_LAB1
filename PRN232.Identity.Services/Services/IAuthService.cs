namespace PRN232.Identity.Services;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default);

    Task<AuthResponse> RefreshAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UserResponse>> GetUsersAsync(
        CancellationToken cancellationToken = default);
}
