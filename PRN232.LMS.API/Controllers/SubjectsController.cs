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
[Route("api/subjects")]
[Route("api/v{version:apiVersion}/subjects")]
[Produces("application/json", "application/xml")]
public class SubjectsController(ISubjectService subjectService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<object>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<object>>>> GetSubjects(
        [FromQuery] CollectionQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        PagedResult<object> result = await subjectService.GetAsync(parameters, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<object>>.Ok(result.Items, "Subjects retrieved successfully.", result.Pagination));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<SubjectResponse>>> GetSubjectById(
        [FromRoute] int id,
        [FromQuery] string? expand,
        CancellationToken cancellationToken)
    {
        SubjectResponse response = await subjectService.GetByIdAsync(id, expand, cancellationToken);
        return Ok(ApiResponse<SubjectResponse>.Ok(response, "Subject retrieved successfully."));
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<SubjectResponse>>> CreateSubject(
        [FromBody] CreateSubjectRequest request,
        CancellationToken cancellationToken)
    {
        SubjectResponse response = await subjectService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(
            nameof(GetSubjectById),
            new { id = response.SubjectId },
            ApiResponse<SubjectResponse>.Ok(response, "Subject created successfully."));
    }

    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<SubjectResponse>>> UpdateSubject(
        [FromRoute] int id,
        [FromBody] UpdateSubjectRequest request,
        CancellationToken cancellationToken)
    {
        SubjectResponse response = await subjectService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<SubjectResponse>.Ok(response, "Subject updated successfully."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteSubject([FromRoute] int id, CancellationToken cancellationToken)
    {
        await subjectService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse<object?>.Ok(null, "Subject deleted successfully."));
    }
}
