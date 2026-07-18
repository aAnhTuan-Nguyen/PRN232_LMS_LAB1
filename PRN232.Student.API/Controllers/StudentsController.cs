using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.Student.Services;

namespace PRN232.Student.API;

[ApiController]
[Route("api/students")]
[Route("api/v1/students")]
public sealed class StudentsController(IStudentService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<object>>>> Get(
        [FromQuery] CollectionQueryParameters parameters,
        CancellationToken ct)
    {
        var result = await service.GetAsync(parameters, ct);
        return Ok(ApiResponse<IReadOnlyList<object>>.Ok(
            result.Items,
            "Students retrieved successfully.",
            result.Pagination));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<StudentResponse>>> GetById(
        int id,
        [FromQuery] string? expand,
        CancellationToken ct)
    {
        var student = await service.GetByIdAsync(id, expand, ct);
        return Ok(ApiResponse<StudentResponse>.Ok(student, "Student retrieved successfully."));
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<StudentResponse>>> Create(
        CreateStudentRequest request,
        CancellationToken ct)
    {
        var student = await service.CreateAsync(request, ct);
        return CreatedAtAction(
            nameof(GetById),
            new { id = student.StudentId },
            ApiResponse<StudentResponse>.Ok(student, "Student created successfully."));
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<StudentResponse>>> Update(
        int id,
        UpdateStudentRequest request,
        CancellationToken ct)
    {
        var student = await service.UpdateAsync(id, request, ct);
        return Ok(ApiResponse<StudentResponse>.Ok(student, "Student updated successfully."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(int id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return Ok(ApiResponse<object?>.Ok(null, "Student deleted successfully."));
    }
}
