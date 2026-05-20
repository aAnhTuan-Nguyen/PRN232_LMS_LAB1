using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Responses;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;
using PRN232.LMS.Services.Services;

namespace PRN232.LMS.API.Controllers;

[ApiController]
[Route("api/semesters")]
[Produces("application/json")]
public class SemestersController(ISemesterService semesterService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<object>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<object>>>> GetSemesters(
        [FromQuery] CollectionQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        PagedResult<object> result = await semesterService.GetAsync(parameters, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<object>>.Ok(result.Items, "Semesters retrieved successfully.", result.Pagination));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<SemesterResponse>>> GetSemesterById(
        int id,
        [FromQuery] string? expand,
        CancellationToken cancellationToken)
    {
        SemesterResponse response = await semesterService.GetByIdAsync(id, expand, cancellationToken);
        return Ok(ApiResponse<SemesterResponse>.Ok(response, "Semester retrieved successfully."));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<SemesterResponse>>> CreateSemester(
        [FromBody] CreateSemesterRequest request,
        CancellationToken cancellationToken)
    {
        SemesterResponse response = await semesterService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(
            nameof(GetSemesterById),
            new { id = response.SemesterId },
            ApiResponse<SemesterResponse>.Ok(response, "Semester created successfully."));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<SemesterResponse>>> UpdateSemester(
        int id,
        [FromBody] UpdateSemesterRequest request,
        CancellationToken cancellationToken)
    {
        SemesterResponse response = await semesterService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<SemesterResponse>.Ok(response, "Semester updated successfully."));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteSemester(int id, CancellationToken cancellationToken)
    {
        await semesterService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse<object?>.Ok(null, "Semester deleted successfully."));
    }
}
