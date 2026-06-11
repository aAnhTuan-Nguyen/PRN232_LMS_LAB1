using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Responses;
using PRN232.LMS.Services.Models.Responses;
using PRN232.LMS.Services.Services;

namespace PRN232.LMS.API.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/users")]
[Route("api/v{version:apiVersion}/users")]
[Produces("application/json", "application/xml")]
[Authorize(Roles = "Admin")]
public class UsersController(IAuthService authService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<UserResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<UserResponse>>>> GetUsers(CancellationToken cancellationToken)
    {
        IReadOnlyList<UserResponse> response = await authService.GetUsersAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<UserResponse>>.Ok(response, "Users retrieved successfully."));
    }
}
