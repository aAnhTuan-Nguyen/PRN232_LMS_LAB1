using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.Identity.Services;

namespace PRN232.Identity.API;

[ApiController]
[Route("api/users")]
[Route("api/v1/users")]
[Authorize(Roles = "Admin")]
public sealed class UsersController(IAuthService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<UserResponse>>>> Get(CancellationToken ct)
    {
        var users = await service.GetUsersAsync(ct);
        return Ok(ApiResponse<IReadOnlyList<UserResponse>>.Ok(users, "Users retrieved successfully."));
    }
}
