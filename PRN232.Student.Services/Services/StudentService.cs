using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Contracts;
using PRN232.Student.Repositories;
using StudentEntity = PRN232.Student.Repositories.Student;

namespace PRN232.Student.Services;

public sealed class StudentService(
    IUnitOfWork unitOfWork,
    CourseEnrollmentLookup.CourseEnrollmentLookupClient courseClient) : IStudentService
{
    public async Task<PagedResult> GetAsync(CollectionQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        var query = unitOfWork.Students.Query().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.Trim().ToLower();
            query = query.Where(x => x.FullName.ToLower().Contains(search) || x.Email.ToLower().Contains(search));
        }

        query = parameters.Sort?.StartsWith("-") == true
            ? query.OrderByDescending(x => x.FullName)
            : query.OrderBy(x => x.FullName);

        var total = await query.CountAsync(cancellationToken);
        var page = Math.Max(1, parameters.Page);
        var size = Math.Clamp(parameters.Size, 1, 100);
        var students = await query.Skip((page - 1) * size).Take(size).ToListAsync(cancellationToken);
        var enrollments = await GetEnrollmentsAsync(students.Select(x => x.StudentId), parameters.Expand, cancellationToken);

        return new PagedResult
        {
            Items = students.Select(x => Select(
                    Map(x, enrollments.TryGetValue(x.StudentId, out var value) ? value : []),
                    parameters.Fields))
                .ToList(),
            Pagination = new PaginationMetadata
            {
                Page = page,
                PageSize = size,
                TotalItems = total,
                TotalPages = (int)Math.Ceiling(total / (double)size)
            }
        };
    }

    public async Task<StudentResponse> GetByIdAsync(int id, string? expand, CancellationToken cancellationToken = default)
    {
        var student = await unitOfWork.Students.Query().AsNoTracking()
                          .SingleOrDefaultAsync(x => x.StudentId == id, cancellationToken)
                      ?? throw new NotFoundException($"Student with id {id} was not found.");
        var enrollments = await GetEnrollmentsAsync([id], expand, cancellationToken);

        return Map(student, enrollments.TryGetValue(id, out var value) ? value : []);
    }

    public async Task<StudentResponse> CreateAsync(CreateStudentRequest request, CancellationToken cancellationToken = default)
    {
        if (await unitOfWork.Students.Query().AnyAsync(x => x.Email == request.Email.Trim(), cancellationToken))
        {
            throw new BadRequestException("Email is already in use.");
        }

        var student = new StudentEntity
        {
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            DateOfBirth = request.DateOfBirth
        };
        await unitOfWork.Students.AddAsync(student, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(student, []);
    }

    public async Task<StudentResponse> UpdateAsync(int id, UpdateStudentRequest request, CancellationToken cancellationToken = default)
    {
        var student = await unitOfWork.Students.GetByIdAsync(id, cancellationToken)
                      ?? throw new NotFoundException($"Student with id {id} was not found.");

        if (await unitOfWork.Students.Query().AnyAsync(
                x => x.StudentId != id && x.Email == request.Email.Trim(),
                cancellationToken))
        {
            throw new BadRequestException("Email is already in use.");
        }

        student.FullName = request.FullName.Trim();
        student.Email = request.Email.Trim().ToLowerInvariant();
        student.DateOfBirth = request.DateOfBirth;
        unitOfWork.Students.Update(student);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(student, []);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var student = await unitOfWork.Students.GetByIdAsync(id, cancellationToken)
                      ?? throw new NotFoundException($"Student with id {id} was not found.");
        var response = await courseClient.HasEnrollmentsAsync(
            new StudentIdRequest { StudentId = id },
            cancellationToken: cancellationToken);

        if (response.Exists)
        {
            throw new BadRequestException("Student cannot be deleted while enrollments exist.");
        }

        unitOfWork.Students.Remove(student);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Dictionary<int, IReadOnlyList<EnrollmentSummary>>> GetEnrollmentsAsync(
        IEnumerable<int> ids,
        string? expand,
        CancellationToken cancellationToken)
    {
        if (!HasExpand(expand, "enrollments"))
        {
            return [];
        }

        var request = new StudentIdsRequest();
        request.StudentIds.AddRange(ids);
        var reply = await courseClient.GetEnrollmentsAsync(request, cancellationToken: cancellationToken);

        return reply.Enrollments
            .GroupBy(x => x.StudentId)
            .ToDictionary(
                x => x.Key,
                x => (IReadOnlyList<EnrollmentSummary>)x.Select(enrollment => new EnrollmentSummary
                {
                    EnrollmentId = enrollment.EnrollmentId,
                    CourseId = enrollment.CourseId,
                    CourseName = enrollment.CourseName,
                    EnrollDate = DateTime.Parse(enrollment.EnrollDate),
                    Status = enrollment.Status
                }).ToList());
    }

    private static StudentResponse Map(StudentEntity student, IReadOnlyList<EnrollmentSummary> enrollments)
    {
        return new StudentResponse
        {
            StudentId = student.StudentId,
            FullName = student.FullName,
            Email = student.Email,
            DateOfBirth = student.DateOfBirth,
            Enrollments = enrollments.Count == 0 ? null : enrollments
        };
    }

    private static bool HasExpand(string? value, string name)
    {
        return value?.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Contains(name, StringComparer.OrdinalIgnoreCase) == true;
    }

    private static object Select(StudentResponse value, string? fields)
    {
        if (string.IsNullOrWhiteSpace(fields))
        {
            return value;
        }

        var names = fields.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return typeof(StudentResponse).GetProperties()
            .Where(property => names.Contains(property.Name))
            .ToDictionary(
                property => char.ToLowerInvariant(property.Name[0]) + property.Name[1..],
                property => property.GetValue(value));
    }
}
