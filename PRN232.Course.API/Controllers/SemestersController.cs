using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.Course.Services;

namespace PRN232.Course.API;

[ApiController]
[Route("api/semesters")]
[Route("api/v1/semesters")]
public sealed class SemestersController(ICourseLmsService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<object>>>> Get(
        [FromQuery] CollectionQueryParameters parameters,
        CancellationToken ct)
    {
        var result = await service.GetSemestersAsync(parameters, ct);
        return Ok(ApiResponse<IReadOnlyList<object>>.Ok(result.Items, "Semesters retrieved successfully.", result.Pagination));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<SemesterResponse>>> GetById(
        int id,
        [FromQuery] string? expand,
        CancellationToken ct)
    {
        var semester = await service.GetSemesterAsync(id, expand, ct);
        return Ok(ApiResponse<SemesterResponse>.Ok(semester, "Semester retrieved successfully."));
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<SemesterResponse>>> Create(
        SemesterRequest request,
        CancellationToken ct)
    {
        var semester = await service.SaveSemesterAsync(null, request, ct);
        return CreatedAtAction(
            nameof(GetById),
            new { id = semester.SemesterId },
            ApiResponse<SemesterResponse>.Ok(semester, "Semester created successfully."));
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<SemesterResponse>>> Update(
        int id,
        SemesterRequest request,
        CancellationToken ct)
    {
        var semester = await service.SaveSemesterAsync(id, request, ct);
        return Ok(ApiResponse<SemesterResponse>.Ok(semester, "Semester updated successfully."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(
        int id,
        CancellationToken ct)
    {
        await service.DeleteSemesterAsync(id, ct);
        return Ok(ApiResponse<object?>.Ok(null, "Semester deleted successfully."));
    }
}
