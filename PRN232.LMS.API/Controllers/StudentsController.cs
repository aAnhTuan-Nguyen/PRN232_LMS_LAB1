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
[ApiVersion(2.0)]
[Route("api/students")]
[Produces("application/json", "application/xml")]
public class StudentsController(IStudentService studentService) : ControllerBase
{
    [HttpGet]
    [HttpGet("/api/v{version:apiVersion}/students")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<object>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<object>>>> GetStudents(
        [FromQuery] CollectionQueryParameters parameters,
        [FromHeader(Name = "X-Request-Id")] string? requestId,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(requestId))
        {
            Response.Headers["X-Request-Id"] = requestId;
        }

        PagedResult<object> result = await studentService.GetAsync(parameters, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<object>>.Ok(result.Items, "Students retrieved successfully.", result.Pagination));
    }

    [HttpGet("{id:int}", Name = "GetStudentById")]
    [HttpGet("/api/v{version:apiVersion}/students/{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<StudentResponse>>> GetStudentById(
        [FromRoute] int id,
        [FromQuery] string? expand,
        CancellationToken cancellationToken)
    {
        StudentResponse response = await studentService.GetByIdAsync(id, expand, cancellationToken);
        return Ok(ApiResponse<StudentResponse>.Ok(response, "Student retrieved successfully."));
    }

    [HttpPost]
    [HttpPost("/api/v{version:apiVersion}/students")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<StudentResponse>>> CreateStudent(
        [FromBody] CreateStudentRequest request,
        CancellationToken cancellationToken)
    {
        StudentResponse response = await studentService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(
            nameof(GetStudentById),
            new { id = response.StudentId },
            ApiResponse<StudentResponse>.Ok(response, "Student created successfully."));
    }

    [HttpPut("{id:int}")]
    [HttpPut("/api/v{version:apiVersion}/students/{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<StudentResponse>>> UpdateStudent(
        [FromRoute] int id,
        [FromBody] UpdateStudentRequest request,
        CancellationToken cancellationToken)
    {
        StudentResponse response = await studentService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<StudentResponse>.Ok(response, "Student updated successfully."));
    }

    [HttpDelete("{id:int}")]
    [HttpDelete("/api/v{version:apiVersion}/students/{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteStudent([FromRoute] int id, CancellationToken cancellationToken)
    {
        await studentService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse<object?>.Ok(null, "Student deleted successfully."));
    }
}
