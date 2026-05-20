using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;

namespace PRN232.LMS.Services.Services;

public interface ISubjectService
{
    Task<PagedResult<object>> GetAsync(CollectionQueryParameters parameters, CancellationToken cancellationToken = default);

    Task<SubjectResponse> GetByIdAsync(int id, string? expand = null, CancellationToken cancellationToken = default);

    Task<SubjectResponse> CreateAsync(CreateSubjectRequest request, CancellationToken cancellationToken = default);

    Task<SubjectResponse> UpdateAsync(int id, UpdateSubjectRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
