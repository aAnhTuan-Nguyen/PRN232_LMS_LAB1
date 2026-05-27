using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Responses;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;
using PRN232.LMS.Services.Services;

namespace PRN232.LMS.API.Controllers;

[ApiController]
[Route("api/courses")]
[Produces("application/json")]
public class CoursesController(ICourseService courseService, IEnrollmentService enrollmentService) : ControllerBase
{

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<object>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<object>>>> GetCourses(
        [FromQuery] CollectionQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        PagedResult<object> result = await courseService.GetAsync(parameters, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<object>>.Ok(result.Items, "Courses retrieved successfully.", result.Pagination));
    }

    
    /// <param name="id">Course id.</param>
    /// <param name="expand">Comma-separated related data to include. Supported values: semester, subject, enrollments.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> GetCourseById(
        int id,
        [FromQuery] string? expand,
        CancellationToken cancellationToken)
    {
        CourseResponse response = await courseService.GetByIdAsync(id, expand, cancellationToken);
        return Ok(ApiResponse<CourseResponse>.Ok(response, "Course retrieved successfully."));
    }

    /// <param name="id">Course id used to limit enrollments.</param>
    /// <param name="parameters">Collection query options. Use expand=student to include student summaries.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    [HttpGet("{id:int}/enrollments")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<object>>>> GetCourseEnrollments(
        int id,
        [FromQuery] CollectionQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        PagedResult<object> result = await enrollmentService.GetByCourseAsync(id, parameters, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<object>>.Ok(result.Items, "Course enrollments retrieved successfully.", result.Pagination));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> CreateCourse(
        [FromBody] CreateCourseRequest request,
        CancellationToken cancellationToken)
    {
        CourseResponse response = await courseService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(
            nameof(GetCourseById),
            new { id = response.CourseId },
            ApiResponse<CourseResponse>.Ok(response, "Course created successfully."));
    }

    /// <param name="id">Course id.</param>
    /// <param name="request">Updated course data.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> UpdateCourse(
        int id,
        [FromBody] UpdateCourseRequest request,
        CancellationToken cancellationToken)
    {
        CourseResponse response = await courseService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<CourseResponse>.Ok(response, "Course updated successfully."));
    }


    /// <param name="id">Course id.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteCourse(int id, CancellationToken cancellationToken)
    {
        await courseService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse<object?>.Ok(null, "Course deleted successfully."));
    }
}
