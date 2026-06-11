using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Responses;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;
using PRN232.LMS.Services.Services;

namespace PRN232.LMS.API.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/enrollments")]
[Route("api/v{version:apiVersion}/enrollments")]
[Produces("application/json", "application/xml")]
public class EnrollmentsController(IEnrollmentService enrollmentService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<object>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<object>>>> GetEnrollments(
        [FromQuery] CollectionQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        PagedResult<object> result = await enrollmentService.GetAsync(parameters, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<object>>.Ok(result.Items, "Enrollments retrieved successfully.", result.Pagination));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EnrollmentResponse>>> GetEnrollmentById(
        [FromRoute] int id,
        [FromQuery] string? expand,
        CancellationToken cancellationToken)
    {
        EnrollmentResponse response = await enrollmentService.GetByIdAsync(id, expand, cancellationToken);
        return Ok(ApiResponse<EnrollmentResponse>.Ok(response, "Enrollment retrieved successfully."));
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EnrollmentResponse>>> CreateEnrollment(
        [FromBody] CreateEnrollmentRequest request,
        CancellationToken cancellationToken)
    {
        EnrollmentResponse response = await enrollmentService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(
            nameof(GetEnrollmentById),
            new { id = response.EnrollmentId },
            ApiResponse<EnrollmentResponse>.Ok(response, "Enrollment created successfully."));
    }

    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EnrollmentResponse>>> UpdateEnrollment(
        [FromRoute] int id,
        [FromBody] UpdateEnrollmentRequest request,
        CancellationToken cancellationToken)
    {
        EnrollmentResponse response = await enrollmentService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<EnrollmentResponse>.Ok(response, "Enrollment updated successfully."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteEnrollment([FromRoute] int id, CancellationToken cancellationToken)
    {
        await enrollmentService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse<object?>.Ok(null, "Enrollment deleted successfully."));
    }
}
