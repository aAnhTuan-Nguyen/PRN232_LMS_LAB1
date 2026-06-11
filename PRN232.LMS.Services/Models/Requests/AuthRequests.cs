using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.Services.Models.Requests;

public class LoginRequest
{
    [Required]
    [StringLength(50)]
    [RegularExpression(@"^[a-zA-Z0-9._-]+$")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
}

public class RefreshTokenRequest
{
    [Required]
    [StringLength(256)]
    public string RefreshToken { get; set; } = string.Empty;
}
