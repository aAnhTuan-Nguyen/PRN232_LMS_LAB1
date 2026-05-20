using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;

namespace PRN232.LMS.Services.Services;

public interface ISemesterService
{
    Task<PagedResult<object>> GetAsync(CollectionQueryParameters parameters, CancellationToken cancellationToken = default);

    Task<SemesterResponse> GetByIdAsync(int id, string? expand = null, CancellationToken cancellationToken = default);

    Task<SemesterResponse> CreateAsync(CreateSemesterRequest request, CancellationToken cancellationToken = default);

    Task<SemesterResponse> UpdateAsync(int id, UpdateSemesterRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
