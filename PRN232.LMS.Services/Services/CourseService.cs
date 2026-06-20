using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.UnitOfWork;
using PRN232.LMS.Services.Exceptions;
using PRN232.LMS.Services.Models.Business;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;

namespace PRN232.LMS.Services.Services;

public class CourseService(IUnitOfWork unitOfWork) : ICourseService
{
    private static readonly IReadOnlyDictionary<string, LambdaExpression> Sorts =
        new Dictionary<string, LambdaExpression>(StringComparer.OrdinalIgnoreCase)
        {
            ["courseId"] = (Expression<Func<Course, int>>)(course => course.CourseId),
            ["courseName"] = (Expression<Func<Course, string>>)(course => course.CourseName),
            ["semesterId"] = (Expression<Func<Course, int>>)(course => course.SemesterId),
            ["subjectId"] = (Expression<Func<Course, int>>)(course => course.SubjectId)
        };

    public Task<PagedResult<object>> GetAsync(CollectionQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        bool includeSemester = QueryHelpers.HasExpand(parameters.Expand, "semester");
        bool includeSubject = QueryHelpers.HasExpand(parameters.Expand, "subject");
        bool includeEnrollments = QueryHelpers.HasExpand(parameters.Expand, "enrollments");
        IQueryable<Course> query = unitOfWork.Courses.Query().AsNoTracking();

        if (includeSemester)
        {
            query = query.Include(course => course.Semester);
        }

        if (includeSubject)
        {
            query = query.Include(course => course.Subject);
        }

        if (includeEnrollments)
        {
            query = query.Include(course => course.Enrollments);
        }

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            string search = parameters.Search.Trim().ToLower();
            query = query.Where(course =>
                course.CourseName.ToLower().Contains(search)
                || course.Semester.SemesterName.ToLower().Contains(search)
                || course.Subject.SubjectName.ToLower().Contains(search)
                || course.Subject.SubjectCode.ToLower().Contains(search));
        }

        query = QueryHelpers.ApplySort(query, parameters.Sort, Sorts, "courseId");
        return QueryHelpers.ToPagedResponseAsync(
            query,
            parameters,
            course =>
            {
                CourseBusinessModel businessModel = LmsMapping.ToCourseBusinessModel(
                    course,
                    includeSemester,
                    includeSubject,
                    includeEnrollments);
                return LmsMapping.ToCourseResponse(businessModel, includeSemester, includeSubject, includeEnrollments);
            },
            cancellationToken);
    }

    public async Task<CourseResponse> GetByIdAsync(int id, string? expand = null, CancellationToken cancellationToken = default)
    {
        bool includeSemester = QueryHelpers.HasExpand(expand, "semester");
        bool includeSubject = QueryHelpers.HasExpand(expand, "subject");
        bool includeEnrollments = QueryHelpers.HasExpand(expand, "enrollments");
        IQueryable<Course> query = unitOfWork.Courses.Query().AsNoTracking();

        if (includeSemester)
        {
            query = query.Include(item => item.Semester);
        }

        if (includeSubject)
        {
            query = query.Include(item => item.Subject);
        }

        if (includeEnrollments)
        {
            query = query.Include(item => item.Enrollments);
        }

        Course course = await query.SingleOrDefaultAsync(item => item.CourseId == id, cancellationToken)
            ?? throw new NotFoundException($"Course with id {id} was not found.");

        CourseBusinessModel businessModel = LmsMapping.ToCourseBusinessModel(
            course,
            includeSemester,
            includeSubject,
            includeEnrollments);
        return LmsMapping.ToCourseResponse(businessModel, includeSemester, includeSubject, includeEnrollments);
    }

    public async Task<CourseResponse> CreateAsync(CreateCourseRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureSemesterExistsAsync(request.SemesterId, cancellationToken);
        await EnsureSubjectExistsAsync(request.SubjectId, cancellationToken);

        Course course = new()
        {
            CourseName = request.CourseName.Trim(),
            SemesterId = request.SemesterId,
            SubjectId = request.SubjectId
        };

        await unitOfWork.Courses.AddAsync(course, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(course.CourseId, cancellationToken: cancellationToken);
    }

    public async Task<CourseResponse> UpdateAsync(int id, UpdateCourseRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureSemesterExistsAsync(request.SemesterId, cancellationToken);
        await EnsureSubjectExistsAsync(request.SubjectId, cancellationToken);

        Course course = await unitOfWork.Courses.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Course with id {id} was not found.");

        course.CourseName = request.CourseName.Trim();
        course.SemesterId = request.SemesterId;
        course.SubjectId = request.SubjectId;

        unitOfWork.Courses.Update(course);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        Course course = await unitOfWork.Courses.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Course with id {id} was not found.");

        unitOfWork.Courses.Remove(course);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureSemesterExistsAsync(int semesterId, CancellationToken cancellationToken)
    {
        bool exists = await unitOfWork.Semesters.Query()
            .AnyAsync(semester => semester.SemesterId == semesterId, cancellationToken);

        if (!exists)
        {
            throw new NotFoundException($"Semester with id {semesterId} was not found.");
        }
    }

    private async Task EnsureSubjectExistsAsync(int subjectId, CancellationToken cancellationToken)
    {
        bool exists = await unitOfWork.Subjects.Query()
            .AnyAsync(subject => subject.SubjectId == subjectId, cancellationToken);

        if (!exists)
        {
            throw new NotFoundException($"Subject with id {subjectId} was not found.");
        }
    }
}
