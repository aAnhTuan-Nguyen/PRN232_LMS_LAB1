namespace PRN232.Identity.Services;

public sealed class UserResponse
{
    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}
