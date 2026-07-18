namespace PRN232.Student.Services;

public interface IStudentService
{
    Task<PagedResult> GetAsync(CollectionQueryParameters parameters, CancellationToken cancellationToken = default);
    Task<StudentResponse> GetByIdAsync(int id, string? expand, CancellationToken cancellationToken = default);
    Task<StudentResponse> CreateAsync(CreateStudentRequest request, CancellationToken cancellationToken = default);
    Task<StudentResponse> UpdateAsync(int id, UpdateStudentRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
