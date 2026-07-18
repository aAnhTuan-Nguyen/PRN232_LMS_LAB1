namespace PRN232.Identity.Services;

public sealed class LoginRequest
{
    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
