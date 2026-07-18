namespace PRN232.Identity.Services;

public sealed class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
