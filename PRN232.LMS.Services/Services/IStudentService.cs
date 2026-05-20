using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;

namespace PRN232.LMS.Services.Services;

public interface IStudentService
{
    Task<PagedResult<object>> GetAsync(CollectionQueryParameters parameters, CancellationToken cancellationToken = default);

    Task<StudentResponse> GetByIdAsync(int id, string? expand = null, CancellationToken cancellationToken = default);

    Task<StudentResponse> CreateAsync(CreateStudentRequest request, CancellationToken cancellationToken = default);

    Task<StudentResponse> UpdateAsync(int id, UpdateStudentRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
