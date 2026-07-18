using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PRN232.Course.Repositories;

public sealed class Semester { public int SemesterId { get; set; } public string SemesterName { get; set; } = string.Empty; public DateTime StartDate { get; set; } public DateTime EndDate { get; set; } public ICollection<Course> Courses { get; set; } = new List<Course>(); }
public sealed class Subject { public int SubjectId { get; set; } public string SubjectCode { get; set; } = string.Empty; public string SubjectName { get; set; } = string.Empty; public int Credit { get; set; } public ICollection<Course> Courses { get; set; } = new List<Course>(); }
public sealed class Course { public int CourseId { get; set; } public string CourseName { get; set; } = string.Empty; public int SemesterId { get; set; } public Semester Semester { get; set; } = null!; public int SubjectId { get; set; } public Subject Subject { get; set; } = null!; public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>(); }
public sealed class Enrollment { public int EnrollmentId { get; set; } public int StudentId { get; set; } public int CourseId { get; set; } public Course Course { get; set; } = null!; public DateTime EnrollDate { get; set; } public string Status { get; set; } = string.Empty; }

public sealed class CourseDbContext(DbContextOptions<CourseDbContext> options) : DbContext(options)
{
    public DbSet<Semester> Semesters => Set<Semester>(); public DbSet<Subject> Subjects => Set<Subject>(); public DbSet<Course> Courses => Set<Course>(); public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Semester>(e => { e.ToTable("Semester"); e.HasKey(x => x.SemesterId); e.Property(x => x.SemesterName).HasMaxLength(100).IsRequired(); });
        modelBuilder.Entity<Subject>(e => { e.ToTable("Subject"); e.HasKey(x => x.SubjectId); e.Property(x => x.SubjectCode).HasMaxLength(20).IsRequired(); e.Property(x => x.SubjectName).HasMaxLength(100).IsRequired(); e.HasIndex(x => x.SubjectCode).IsUnique(); });
        modelBuilder.Entity<Course>(e => { e.ToTable("Course"); e.HasKey(x => x.CourseId); e.Property(x => x.CourseName).HasMaxLength(100).IsRequired(); e.HasOne(x => x.Semester).WithMany(x => x.Courses).HasForeignKey(x => x.SemesterId).OnDelete(DeleteBehavior.Restrict); e.HasOne(x => x.Subject).WithMany(x => x.Courses).HasForeignKey(x => x.SubjectId).OnDelete(DeleteBehavior.Restrict); });
        modelBuilder.Entity<Enrollment>(e => { e.ToTable("Enrollment"); e.HasKey(x => x.EnrollmentId); e.Property(x => x.Status).HasMaxLength(20).IsRequired(); e.HasIndex(x => x.StudentId); e.HasOne(x => x.Course).WithMany(x => x.Enrollments).HasForeignKey(x => x.CourseId).OnDelete(DeleteBehavior.Restrict); });
        modelBuilder.Entity<Semester>().HasData(Enumerable.Range(1, 5).Select(i => new Semester { SemesterId = i, SemesterName = $"Semester {i}", StartDate = new DateTime(2026, (i - 1) * 2 + 1, 1, 0, 0, 0, DateTimeKind.Utc), EndDate = new DateTime(2026, (i - 1) * 2 + 2, 28, 0, 0, 0, DateTimeKind.Utc) }));
        var subjects = new[] { "Programming C#", "REST API Basics", "Database Systems", "Web Application Development", "Software Testing", "Cloud Deployment", "Object-Oriented Programming", "Data Structures", "Software Architecture", "Project Management" };
        modelBuilder.Entity<Subject>().HasData(subjects.Select((name, i) => new Subject { SubjectId = i + 1, SubjectCode = $"PRN{232 + i}", SubjectName = name, Credit = i % 3 + 2 }));
        modelBuilder.Entity<Course>().HasData(Enumerable.Range(1, 20).Select(i => new Course { CourseId = i, CourseName = $"LMS Course {i:00}", SemesterId = (i - 1) % 5 + 1, SubjectId = (i - 1) % 10 + 1 }));
        var statuses = new[] { "Active", "Completed", "Dropped", "Pending" };
        modelBuilder.Entity<Enrollment>().HasData(Enumerable.Range(1, 500).Select(i => new Enrollment { EnrollmentId = i, StudentId = (i - 1) % 50 + 1, CourseId = ((i - 1) * 7) % 20 + 1, EnrollDate = new DateTime(2026, (i - 1) % 12 + 1, (i - 1) % 27 + 1, 0, 0, 0, DateTimeKind.Utc), Status = statuses[(i - 1) % statuses.Length] }));
    }
}

public interface IGenericRepository<TEntity> where TEntity : class { IQueryable<TEntity> Query(); Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default); Task AddAsync(TEntity entity, CancellationToken cancellationToken = default); void Update(TEntity entity); void Remove(TEntity entity); }
internal sealed class GenericRepository<TEntity>(CourseDbContext context) : IGenericRepository<TEntity> where TEntity : class
{
    public IQueryable<TEntity> Query() => context.Set<TEntity>(); public async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => await context.Set<TEntity>().FindAsync([id], cancellationToken); public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) => context.Set<TEntity>().AddAsync(entity, cancellationToken).AsTask(); public void Update(TEntity entity) => context.Set<TEntity>().Update(entity); public void Remove(TEntity entity) => context.Set<TEntity>().Remove(entity);
}
public interface IUnitOfWork { IGenericRepository<Semester> Semesters { get; } IGenericRepository<Subject> Subjects { get; } IGenericRepository<Course> Courses { get; } IGenericRepository<Enrollment> Enrollments { get; } Task<int> SaveChangesAsync(CancellationToken cancellationToken = default); }
public sealed class UnitOfWork(CourseDbContext context) : IUnitOfWork
{
    public IGenericRepository<Semester> Semesters { get; } = new GenericRepository<Semester>(context); public IGenericRepository<Subject> Subjects { get; } = new GenericRepository<Subject>(context); public IGenericRepository<Course> Courses { get; } = new GenericRepository<Course>(context); public IGenericRepository<Enrollment> Enrollments { get; } = new GenericRepository<Enrollment>(context); public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => context.SaveChangesAsync(cancellationToken);
}
public sealed class CourseDesignTimeFactory : IDesignTimeDbContextFactory<CourseDbContext> { public CourseDbContext CreateDbContext(string[] args) => new(new DbContextOptionsBuilder<CourseDbContext>().UseNpgsql(Environment.GetEnvironmentVariable("CourseConnection") ?? "Host=localhost;Database=course_lms;Username=postgres;Password=postgres").Options); }
public static class DependencyInjection { public static IServiceCollection AddCourseRepositories(this IServiceCollection services, IConfiguration configuration) { var connection = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required."); services.AddDbContext<CourseDbContext>(options => options.UseNpgsql(connection)); services.AddScoped<IUnitOfWork, UnitOfWork>(); return services; } }
