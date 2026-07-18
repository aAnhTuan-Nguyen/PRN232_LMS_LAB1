using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.Course.Services;

namespace PRN232.Course.API;

[ApiController]
[Route("api/courses")]
[Route("api/v1/courses")]
public sealed class CoursesController(ICourseLmsService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<object>>>> Get(
        [FromQuery] CollectionQueryParameters parameters,
        CancellationToken ct)
    {
        var result = await service.GetCoursesAsync(parameters, ct);
        return Ok(ApiResponse<IReadOnlyList<object>>.Ok(result.Items, "Courses retrieved successfully.", result.Pagination));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> GetById(
        int id,
        [FromQuery] string? expand,
        CancellationToken ct)
    {
        var course = await service.GetCourseAsync(id, expand, ct);
        return Ok(ApiResponse<CourseResponse>.Ok(course, "Course retrieved successfully."));
    }

    [HttpGet("{id:int}/students")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<object>>>> GetStudents(
        int id,
        CancellationToken ct)
    {
        var result = await service.GetEnrollmentsAsync(
            new CollectionQueryParameters { Search = null, Expand = "student", Size = 100, Page = 1 },
            ct);
        var students = result.Items.Where(item => item is EnrollmentResponse enrollment && enrollment.CourseId == id).ToList();

        return Ok(ApiResponse<IReadOnlyList<object>>.Ok(students, "Course students retrieved successfully."));
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> Create(
        CourseRequest request,
        CancellationToken ct)
    {
        var course = await service.SaveCourseAsync(null, request, ct);
        return CreatedAtAction(
            nameof(GetById),
            new { id = course.CourseId },
            ApiResponse<CourseResponse>.Ok(course, "Course created successfully."));
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> Update(
        int id,
        CourseRequest request,
        CancellationToken ct)
    {
        var course = await service.SaveCourseAsync(id, request, ct);
        return Ok(ApiResponse<CourseResponse>.Ok(course, "Course updated successfully."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(
        int id,
        CancellationToken ct)
    {
        await service.DeleteCourseAsync(id, ct);
        return Ok(ApiResponse<object?>.Ok(null, "Course deleted successfully."));
    }
}
