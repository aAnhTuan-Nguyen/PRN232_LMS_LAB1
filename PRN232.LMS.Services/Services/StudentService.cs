using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.UnitOfWork;
using PRN232.LMS.Services.Exceptions;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;

namespace PRN232.LMS.Services.Services;

public class StudentService(IUnitOfWork unitOfWork) : IStudentService
{
    private static readonly IReadOnlyDictionary<string, LambdaExpression> Sorts =
        new Dictionary<string, LambdaExpression>(StringComparer.OrdinalIgnoreCase)
        {
            ["studentId"] = (Expression<Func<Student, int>>)(student => student.StudentId),
            ["fullName"] = (Expression<Func<Student, string>>)(student => student.FullName),
            ["email"] = (Expression<Func<Student, string>>)(student => student.Email),
            ["dateOfBirth"] = (Expression<Func<Student, DateTime>>)(student => student.DateOfBirth)
        };

    public Task<PagedResult<object>> GetAsync(CollectionQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        bool includeEnrollments = QueryHelpers.HasExpand(parameters.Expand, "enrollments");
        IQueryable<Student> query = unitOfWork.Students.Query().AsNoTracking();

        if (includeEnrollments)
        {
            query = query.Include(student => student.Enrollments)
                .ThenInclude(enrollment => enrollment.Course)
                .ThenInclude(course => course.Semester)
                .Include(student => student.Enrollments)
                .ThenInclude(enrollment => enrollment.Course)
                .ThenInclude(course => course.Subject);
        }

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            string search = parameters.Search.Trim().ToLower();
            query = query.Where(student =>
                student.FullName.ToLower().Contains(search)
                || student.Email.ToLower().Contains(search));
        }

        query = QueryHelpers.ApplySort(query, parameters.Sort, Sorts, "studentId");
        return QueryHelpers.ToPagedResponseAsync(
            query,
            parameters,
            student => LmsMapping.ToStudentResponse(student, includeEnrollments),
            cancellationToken);
    }

    public async Task<StudentResponse> GetByIdAsync(int id, string? expand = null, CancellationToken cancellationToken = default)
    {
        bool includeEnrollments = QueryHelpers.HasExpand(expand, "enrollments");
        IQueryable<Student> query = unitOfWork.Students.Query().AsNoTracking();

        if (includeEnrollments)
        {
            query = query.Include(item => item.Enrollments)
                .ThenInclude(enrollment => enrollment.Course)
                .ThenInclude(course => course.Semester)
                .Include(item => item.Enrollments)
                .ThenInclude(enrollment => enrollment.Course)
                .ThenInclude(course => course.Subject);
        }

        Student student = await query.SingleOrDefaultAsync(item => item.StudentId == id, cancellationToken)
            ?? throw new NotFoundException($"Student with id {id} was not found.");

        return LmsMapping.ToStudentResponse(student, includeEnrollments);
    }

    public async Task<StudentResponse> CreateAsync(CreateStudentRequest request, CancellationToken cancellationToken = default)
    {
        Student student = new()
        {
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            DateOfBirth = request.DateOfBirth
        };

        await unitOfWork.Students.AddAsync(student, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return LmsMapping.ToStudentResponse(student, includeEnrollments: false);
    }

    public async Task<StudentResponse> UpdateAsync(int id, UpdateStudentRequest request, CancellationToken cancellationToken = default)
    {
        Student student = await unitOfWork.Students.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Student with id {id} was not found.");

        student.FullName = request.FullName.Trim();
        student.Email = request.Email.Trim();
        student.DateOfBirth = request.DateOfBirth;

        unitOfWork.Students.Update(student);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return LmsMapping.ToStudentResponse(student, includeEnrollments: false);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        Student student = await unitOfWork.Students.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Student with id {id} was not found.");

        unitOfWork.Students.Remove(student);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
