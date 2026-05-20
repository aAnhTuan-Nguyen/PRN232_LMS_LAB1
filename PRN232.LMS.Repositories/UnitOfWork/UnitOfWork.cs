using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Repositories;

namespace PRN232.LMS.Repositories.UnitOfWork;

public class UnitOfWork(LmsDbContext dbContext) : IUnitOfWork
{
    public IGenericRepository<Semester> Semesters { get; } = new GenericRepository<Semester>(dbContext);

    public IGenericRepository<Course> Courses { get; } = new GenericRepository<Course>(dbContext);

    public IGenericRepository<Subject> Subjects { get; } = new GenericRepository<Subject>(dbContext);

    public IGenericRepository<Student> Students { get; } = new GenericRepository<Student>(dbContext);

    public IGenericRepository<Enrollment> Enrollments { get; } = new GenericRepository<Enrollment>(dbContext);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
