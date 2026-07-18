using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.Course.Services;

namespace PRN232.Course.API;

[ApiController]
[Route("api/enrollments")]
[Route("api/v1/enrollments")]
public sealed class EnrollmentsController(ICourseLmsService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<object>>>> Get(
        [FromQuery] CollectionQueryParameters parameters,
        CancellationToken ct)
    {
        var result = await service.GetEnrollmentsAsync(parameters, ct);
        return Ok(ApiResponse<IReadOnlyList<object>>.Ok(result.Items, "Enrollments retrieved successfully.", result.Pagination));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<EnrollmentResponse>>> GetById(
        int id,
        [FromQuery] string? expand,
        CancellationToken ct)
    {
        var enrollment = await service.GetEnrollmentAsync(id, expand, ct);
        return Ok(ApiResponse<EnrollmentResponse>.Ok(enrollment, "Enrollment retrieved successfully."));
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<EnrollmentResponse>>> Create(
        EnrollmentRequest request,
        CancellationToken ct)
    {
        var enrollment = await service.SaveEnrollmentAsync(null, request, ct);
        return CreatedAtAction(
            nameof(GetById),
            new { id = enrollment.EnrollmentId },
            ApiResponse<EnrollmentResponse>.Ok(enrollment, "Enrollment created successfully."));
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<EnrollmentResponse>>> Update(
        int id,
        EnrollmentRequest request,
        CancellationToken ct)
    {
        var enrollment = await service.SaveEnrollmentAsync(id, request, ct);
        return Ok(ApiResponse<EnrollmentResponse>.Ok(enrollment, "Enrollment updated successfully."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(
        int id,
        CancellationToken ct)
    {
        await service.DeleteEnrollmentAsync(id, ct);
        return Ok(ApiResponse<object?>.Ok(null, "Enrollment deleted successfully."));
    }
}
