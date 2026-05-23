using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.UnitOfWork;
using PRN232.LMS.Services.Exceptions;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;

namespace PRN232.LMS.Services.Services;

public class SubjectService(IUnitOfWork unitOfWork) : ISubjectService
{
    private static readonly IReadOnlyDictionary<string, LambdaExpression> Sorts =
        new Dictionary<string, LambdaExpression>(StringComparer.OrdinalIgnoreCase)
        {
            ["subjectId"] = (Expression<Func<Subject, int>>)(subject => subject.SubjectId),
            ["subjectCode"] = (Expression<Func<Subject, string>>)(subject => subject.SubjectCode),
            ["subjectName"] = (Expression<Func<Subject, string>>)(subject => subject.SubjectName),
            ["credit"] = (Expression<Func<Subject, int>>)(subject => subject.Credit)
        };

    public Task<PagedResult<object>> GetAsync(CollectionQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        bool includeCourses = QueryHelpers.HasExpand(parameters.Expand, "courses");
        IQueryable<Subject> query = unitOfWork.Subjects.Query().AsNoTracking();

        if (includeCourses)
        {
            query = query.Include(subject => subject.Courses)
                .ThenInclude(course => course.Semester);
        }

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            string search = parameters.Search.Trim().ToLower();
            query = query.Where(subject =>
                subject.SubjectCode.ToLower().Contains(search)
                || subject.SubjectName.ToLower().Contains(search));
        }

        query = QueryHelpers.ApplySort(query, parameters.Sort, Sorts, "subjectId");
        return QueryHelpers.ToPagedResponseAsync(
            query,
            parameters,
            subject => LmsMapping.ToSubjectResponse(subject, includeCourses),
            cancellationToken);
    }

    public async Task<SubjectResponse> GetByIdAsync(int id, string? expand = null, CancellationToken cancellationToken = default)
    {
        bool includeCourses = QueryHelpers.HasExpand(expand, "courses");
        IQueryable<Subject> query = unitOfWork.Subjects.Query().AsNoTracking();

        if (includeCourses)
        {
            query = query.Include(item => item.Courses)
                .ThenInclude(course => course.Semester);
        }

        Subject subject = await query.SingleOrDefaultAsync(item => item.SubjectId == id, cancellationToken)
            ?? throw new NotFoundException($"Subject with id {id} was not found.");

        return LmsMapping.ToSubjectResponse(subject, includeCourses);
    }

    public async Task<SubjectResponse> CreateAsync(CreateSubjectRequest request, CancellationToken cancellationToken = default)
    {
        Subject subject = new()
        {
            SubjectCode = request.SubjectCode.Trim(),
            SubjectName = request.SubjectName.Trim(),
            Credit = request.Credit
        };

        await unitOfWork.Subjects.AddAsync(subject, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return LmsMapping.ToSubjectResponse(subject, includeCourses: false);
    }

    public async Task<SubjectResponse> UpdateAsync(int id, UpdateSubjectRequest request, CancellationToken cancellationToken = default)
    {
        Subject subject = await unitOfWork.Subjects.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Subject with id {id} was not found.");

        subject.SubjectCode = request.SubjectCode.Trim();
        subject.SubjectName = request.SubjectName.Trim();
        subject.Credit = request.Credit;

        unitOfWork.Subjects.Update(subject);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return LmsMapping.ToSubjectResponse(subject, includeCourses: false);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        Subject subject = await unitOfWork.Subjects.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Subject with id {id} was not found.");

        unitOfWork.Subjects.Remove(subject);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
