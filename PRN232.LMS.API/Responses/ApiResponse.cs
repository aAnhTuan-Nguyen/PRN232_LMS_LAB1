using System.Runtime.Serialization;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Responses;

namespace PRN232.LMS.API.Responses;

[KnownType(typeof(CourseResponse))]
[KnownType(typeof(CourseSummaryResponse))]
[KnownType(typeof(EnrollmentResponse))]
[KnownType(typeof(EnrollmentSummaryResponse))]
[KnownType(typeof(SemesterResponse))]
[KnownType(typeof(SemesterSummaryResponse))]
[KnownType(typeof(StudentResponse))]
[KnownType(typeof(StudentSummaryResponse))]
[KnownType(typeof(SubjectResponse))]
[KnownType(typeof(SubjectSummaryResponse))]
[KnownType(typeof(UserResponse))]
[KnownType(typeof(List<object>))]
public class ApiResponse<T>
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public T? Data { get; set; }

    public object? Errors { get; set; }

    public PaginationMetadata? Pagination { get; set; }

    public static ApiResponse<T> Ok(T? data, string message, PaginationMetadata? pagination = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Errors = null,
            Pagination = pagination
        };
    }

    public static ApiResponse<T> Fail(string message, object? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = default,
            Errors = errors
        };
    }
}
