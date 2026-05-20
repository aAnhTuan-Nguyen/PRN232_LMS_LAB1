using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;

namespace PRN232.LMS.Services.Services;

public interface ICourseService
{
    Task<PagedResult<object>> GetAsync(CollectionQueryParameters parameters, CancellationToken cancellationToken = default);

    Task<CourseResponse> GetByIdAsync(int id, string? expand = null, CancellationToken cancellationToken = default);

    Task<CourseResponse> CreateAsync(CreateCourseRequest request, CancellationToken cancellationToken = default);

    Task<CourseResponse> UpdateAsync(int id, UpdateCourseRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
