using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Repositories;

namespace PRN232.LMS.Repositories.UnitOfWork;

public interface IUnitOfWork
{
    IGenericRepository<Semester> Semesters { get; }

    IGenericRepository<Course> Courses { get; }

    IGenericRepository<Subject> Subjects { get; }

    IGenericRepository<Student> Students { get; }

    IGenericRepository<Enrollment> Enrollments { get; }

    IGenericRepository<User> Users { get; }

    IGenericRepository<RefreshToken> RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
