using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.UnitOfWork;
using PRN232.LMS.Services.Exceptions;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;

namespace PRN232.LMS.Services.Services;

public class EnrollmentService(IUnitOfWork unitOfWork) : IEnrollmentService
{
    private static readonly IReadOnlyDictionary<string, LambdaExpression> Sorts =
        new Dictionary<string, LambdaExpression>(StringComparer.OrdinalIgnoreCase)
        {
            ["enrollmentId"] = (Expression<Func<Enrollment, int>>)(enrollment => enrollment.EnrollmentId),
            ["studentId"] = (Expression<Func<Enrollment, int>>)(enrollment => enrollment.StudentId),
            ["courseId"] = (Expression<Func<Enrollment, int>>)(enrollment => enrollment.CourseId),
            ["enrollDate"] = (Expression<Func<Enrollment, DateTime>>)(enrollment => enrollment.EnrollDate),
            ["status"] = (Expression<Func<Enrollment, string>>)(enrollment => enrollment.Status)
        };

    public Task<PagedResult<object>> GetAsync(CollectionQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        bool includeStudent = QueryHelpers.HasExpand(parameters.Expand, "student");
        bool includeCourse = QueryHelpers.HasExpand(parameters.Expand, "course");
        IQueryable<Enrollment> query = unitOfWork.Enrollments.Query().AsNoTracking();

        if (includeStudent)
        {
            query = query.Include(enrollment => enrollment.Student);
        }

        if (includeCourse)
        {
            query = query.Include(enrollment => enrollment.Course)
                .ThenInclude(course => course.Semester)
                .Include(enrollment => enrollment.Course)
                .ThenInclude(course => course.Subject);
        }

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            string search = parameters.Search.Trim().ToLower();
            query = query.Where(enrollment =>
                enrollment.Status.ToLower().Contains(search)
                || enrollment.Student.FullName.ToLower().Contains(search)
                || enrollment.Student.Email.ToLower().Contains(search)
                || enrollment.Course.CourseName.ToLower().Contains(search));
        }

        query = QueryHelpers.ApplySort(query, parameters.Sort, Sorts, "enrollmentId");
        return QueryHelpers.ToPagedResponseAsync(
            query,
            parameters,
            enrollment => LmsMapping.ToEnrollmentResponse(enrollment, includeStudent, includeCourse),
            cancellationToken);
    }

    public async Task<EnrollmentResponse> GetByIdAsync(int id, string? expand = null, CancellationToken cancellationToken = default)
    {
        bool includeStudent = QueryHelpers.HasExpand(expand, "student");
        bool includeCourse = QueryHelpers.HasExpand(expand, "course");
        IQueryable<Enrollment> query = unitOfWork.Enrollments.Query().AsNoTracking();

        if (includeStudent)
        {
            query = query.Include(item => item.Student);
        }

        if (includeCourse)
        {
            query = query.Include(item => item.Course)
                .ThenInclude(course => course.Semester)
                .Include(item => item.Course)
                .ThenInclude(course => course.Subject);
        }

        Enrollment enrollment = await query.SingleOrDefaultAsync(item => item.EnrollmentId == id, cancellationToken)
            ?? throw new NotFoundException($"Enrollment with id {id} was not found.");

        return LmsMapping.ToEnrollmentResponse(enrollment, includeStudent, includeCourse);
    }

    public async Task<EnrollmentResponse> CreateAsync(CreateEnrollmentRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureStudentExistsAsync(request.StudentId, cancellationToken);
        await EnsureCourseExistsAsync(request.CourseId, cancellationToken);

        Enrollment enrollment = new()
        {
            StudentId = request.StudentId,
            CourseId = request.CourseId,
            EnrollDate = request.EnrollDate,
            Status = request.Status.Trim()
        };

        await unitOfWork.Enrollments.AddAsync(enrollment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(enrollment.EnrollmentId, cancellationToken: cancellationToken);
    }

    public async Task<EnrollmentResponse> UpdateAsync(int id, UpdateEnrollmentRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureStudentExistsAsync(request.StudentId, cancellationToken);
        await EnsureCourseExistsAsync(request.CourseId, cancellationToken);

        Enrollment enrollment = await unitOfWork.Enrollments.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Enrollment with id {id} was not found.");

        enrollment.StudentId = request.StudentId;
        enrollment.CourseId = request.CourseId;
        enrollment.EnrollDate = request.EnrollDate;
        enrollment.Status = request.Status.Trim();

        unitOfWork.Enrollments.Update(enrollment);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        Enrollment enrollment = await unitOfWork.Enrollments.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Enrollment with id {id} was not found.");

        unitOfWork.Enrollments.Remove(enrollment);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureStudentExistsAsync(int studentId, CancellationToken cancellationToken)
    {
        bool exists = await unitOfWork.Students.Query()
            .AnyAsync(student => student.StudentId == studentId, cancellationToken);

        if (!exists)
        {
            throw new NotFoundException($"Student with id {studentId} was not found.");
        }
    }

    private async Task EnsureCourseExistsAsync(int courseId, CancellationToken cancellationToken)
    {
        bool exists = await unitOfWork.Courses.Query()
            .AnyAsync(course => course.CourseId == courseId, cancellationToken);

        if (!exists)
        {
            throw new NotFoundException($"Course with id {courseId} was not found.");
        }
    }
}
