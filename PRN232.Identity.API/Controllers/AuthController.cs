using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.Identity.Services;

namespace PRN232.Identity.API;

[ApiController]
[Route("api/auth")]
[Route("api/v1/auth")]
public sealed class AuthController(IAuthService service) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(
        LoginRequest request,
        CancellationToken ct)
    {
        var response = await service.LoginAsync(request, ct);
        return Ok(ApiResponse<AuthResponse>.Ok(response, "Login successful."));
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Refresh(
        RefreshTokenRequest request,
        CancellationToken ct)
    {
        var response = await service.RefreshAsync(request, ct);
        return Ok(ApiResponse<AuthResponse>.Ok(response, "Token refreshed successfully."));
    }
}
