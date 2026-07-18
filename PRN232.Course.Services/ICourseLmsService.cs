using PRN232.Course.Repositories;

namespace PRN232.Course.Services;

public interface ICourseLmsService
{
    Task<PagedResult> GetSemestersAsync(CollectionQueryParameters p, CancellationToken ct = default); Task<SemesterResponse> GetSemesterAsync(int id, string? expand, CancellationToken ct = default); Task<SemesterResponse> SaveSemesterAsync(int? id, SemesterRequest request, CancellationToken ct = default); Task DeleteSemesterAsync(int id, CancellationToken ct = default);
    Task<PagedResult> GetSubjectsAsync(CollectionQueryParameters p, CancellationToken ct = default); Task<SubjectResponse> GetSubjectAsync(int id, string? expand, CancellationToken ct = default); Task<SubjectResponse> SaveSubjectAsync(int? id, SubjectRequest request, CancellationToken ct = default); Task DeleteSubjectAsync(int id, CancellationToken ct = default);
    Task<PagedResult> GetCoursesAsync(CollectionQueryParameters p, CancellationToken ct = default); Task<CourseResponse> GetCourseAsync(int id, string? expand, CancellationToken ct = default); Task<CourseResponse> SaveCourseAsync(int? id, CourseRequest request, CancellationToken ct = default); Task DeleteCourseAsync(int id, CancellationToken ct = default);
    Task<PagedResult> GetEnrollmentsAsync(CollectionQueryParameters p, CancellationToken ct = default); Task<EnrollmentResponse> GetEnrollmentAsync(int id, string? expand, CancellationToken ct = default); Task<EnrollmentResponse> SaveEnrollmentAsync(int? id, EnrollmentRequest request, CancellationToken ct = default); Task DeleteEnrollmentAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Enrollment>> GetEnrollmentsForStudentsAsync(IReadOnlyCollection<int> ids, CancellationToken ct = default); Task<bool> HasEnrollmentsAsync(int studentId, CancellationToken ct = default);
}
