using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.Course.Services;

namespace PRN232.Course.API;

[ApiController]
[Route("api/subjects")]
[Route("api/v1/subjects")]
public sealed class SubjectsController(ICourseLmsService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<object>>>> Get(
        [FromQuery] CollectionQueryParameters parameters,
        CancellationToken ct)
    {
        var result = await service.GetSubjectsAsync(parameters, ct);
        return Ok(ApiResponse<IReadOnlyList<object>>.Ok(result.Items, "Subjects retrieved successfully.", result.Pagination));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<SubjectResponse>>> GetById(
        int id,
        [FromQuery] string? expand,
        CancellationToken ct)
    {
        var subject = await service.GetSubjectAsync(id, expand, ct);
        return Ok(ApiResponse<SubjectResponse>.Ok(subject, "Subject retrieved successfully."));
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<SubjectResponse>>> Create(
        SubjectRequest request,
        CancellationToken ct)
    {
        var subject = await service.SaveSubjectAsync(null, request, ct);
        return CreatedAtAction(
            nameof(GetById),
            new { id = subject.SubjectId },
            ApiResponse<SubjectResponse>.Ok(subject, "Subject created successfully."));
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<SubjectResponse>>> Update(
        int id,
        SubjectRequest request,
        CancellationToken ct)
    {
        var subject = await service.SaveSubjectAsync(id, request, ct);
        return Ok(ApiResponse<SubjectResponse>.Ok(subject, "Subject updated successfully."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(
        int id,
        CancellationToken ct)
    {
        await service.DeleteSubjectAsync(id, ct);
        return Ok(ApiResponse<object?>.Ok(null, "Subject deleted successfully."));
    }
}
