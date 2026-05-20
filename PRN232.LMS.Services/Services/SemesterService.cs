using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.UnitOfWork;
using PRN232.LMS.Services.Exceptions;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;

namespace PRN232.LMS.Services.Services;

public class SemesterService(IUnitOfWork unitOfWork) : ISemesterService
{
    private static readonly IReadOnlyDictionary<string, LambdaExpression> Sorts =
        new Dictionary<string, LambdaExpression>(StringComparer.OrdinalIgnoreCase)
        {
            ["semesterId"] = (Expression<Func<Semester, int>>)(semester => semester.SemesterId),
            ["semesterName"] = (Expression<Func<Semester, string>>)(semester => semester.SemesterName),
            ["startDate"] = (Expression<Func<Semester, DateTime>>)(semester => semester.StartDate),
            ["endDate"] = (Expression<Func<Semester, DateTime>>)(semester => semester.EndDate)
        };

    public Task<PagedResult<object>> GetAsync(CollectionQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        bool includeCourses = QueryHelpers.HasExpand(parameters.Expand, "courses");
        IQueryable<Semester> query = unitOfWork.Semesters.Query().AsNoTracking();

        if (includeCourses)
        {
            query = query.Include(semester => semester.Courses)
                .ThenInclude(course => course.Subject);
        }

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            string search = parameters.Search.Trim().ToLower();
            query = query.Where(semester => semester.SemesterName.ToLower().Contains(search));
        }

        query = QueryHelpers.ApplySort(query, parameters.Sort, Sorts, "semesterId");
        return QueryHelpers.ToPagedResponseAsync(
            query,
            parameters,
            semester => LmsMapping.ToSemesterResponse(semester, includeCourses),
            cancellationToken);
    }

    public async Task<SemesterResponse> GetByIdAsync(int id, string? expand = null, CancellationToken cancellationToken = default)
    {
        Semester semester = await unitOfWork.Semesters.Query()
            .AsNoTracking()
            .Include(item => item.Courses)
            .ThenInclude(course => course.Subject)
            .SingleOrDefaultAsync(item => item.SemesterId == id, cancellationToken)
            ?? throw new NotFoundException($"Semester with id {id} was not found.");

        return LmsMapping.ToSemesterResponse(semester, includeCourses: true);
    }

    public async Task<SemesterResponse> CreateAsync(CreateSemesterRequest request, CancellationToken cancellationToken = default)
    {
        Semester semester = new()
        {
            SemesterName = request.SemesterName.Trim(),
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };

        await unitOfWork.Semesters.AddAsync(semester, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return LmsMapping.ToSemesterResponse(semester, includeCourses: false);
    }

    public async Task<SemesterResponse> UpdateAsync(int id, UpdateSemesterRequest request, CancellationToken cancellationToken = default)
    {
        Semester semester = await unitOfWork.Semesters.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Semester with id {id} was not found.");

        semester.SemesterName = request.SemesterName.Trim();
        semester.StartDate = request.StartDate;
        semester.EndDate = request.EndDate;

        unitOfWork.Semesters.Update(semester);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return LmsMapping.ToSemesterResponse(semester, includeCourses: false);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        Semester semester = await unitOfWork.Semesters.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Semester with id {id} was not found.");

        unitOfWork.Semesters.Remove(semester);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
