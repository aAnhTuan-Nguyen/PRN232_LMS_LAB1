using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;

namespace PRN232.LMS.Services.Services;

public interface IEnrollmentService
{
    Task<PagedResult<object>> GetAsync(CollectionQueryParameters parameters, CancellationToken cancellationToken = default);

    Task<PagedResult<object>> GetByCourseAsync(int courseId, CollectionQueryParameters parameters, CancellationToken cancellationToken = default);

    Task<EnrollmentResponse> GetByIdAsync(int id, string? expand = null, CancellationToken cancellationToken = default);

    Task<EnrollmentResponse> CreateAsync(CreateEnrollmentRequest request, CancellationToken cancellationToken = default);

    Task<EnrollmentResponse> UpdateAsync(int id, UpdateEnrollmentRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
