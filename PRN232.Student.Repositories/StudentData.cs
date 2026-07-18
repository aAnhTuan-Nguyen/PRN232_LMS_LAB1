using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PRN232.Student.Repositories;

public sealed class Student
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
}

public sealed class StudentDbContext(DbContextOptions<StudentDbContext> options) : DbContext(options)
{
    public DbSet<Student> Students => Set<Student>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("Student");
            entity.HasKey(x => x.StudentId);
            entity.Property(x => x.FullName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(100).IsRequired();
            entity.Property(x => x.DateOfBirth).IsRequired();
            entity.HasIndex(x => x.Email).IsUnique();
        });
        var families = new[] { "Nguyen", "Tran", "Le", "Pham", "Hoang", "Phan", "Vu", "Dang", "Bui", "Do" };
        var names = new[] { "An", "Binh", "Chi", "Dung", "Hanh", "Khoa", "Linh", "Minh", "Nam", "Quyen" };
        modelBuilder.Entity<Student>().HasData(Enumerable.Range(1, 50).Select(i => new Student
        {
            StudentId = i,
            FullName = $"{families[(i - 1) % families.Length]} {names[(i - 1) % names.Length]} {i:00}",
            Email = $"student{i:00}@lms.local",
            DateOfBirth = new DateTime(2000 + i % 5, (i - 1) % 12 + 1, (i - 1) % 27 + 1, 0, 0, 0, DateTimeKind.Utc)
        }));
    }
}

public interface IGenericRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> Query();
    Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}

internal sealed class GenericRepository<TEntity>(StudentDbContext context) : IGenericRepository<TEntity> where TEntity : class
{
    public IQueryable<TEntity> Query() => context.Set<TEntity>();
    public async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => await context.Set<TEntity>().FindAsync([id], cancellationToken);
    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) => context.Set<TEntity>().AddAsync(entity, cancellationToken).AsTask();
    public void Update(TEntity entity) => context.Set<TEntity>().Update(entity);
    public void Remove(TEntity entity) => context.Set<TEntity>().Remove(entity);
}

public interface IUnitOfWork
{
    IGenericRepository<Student> Students { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public sealed class UnitOfWork(StudentDbContext context) : IUnitOfWork
{
    public IGenericRepository<Student> Students { get; } = new GenericRepository<Student>(context);
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => context.SaveChangesAsync(cancellationToken);
}

public sealed class StudentDesignTimeFactory : IDesignTimeDbContextFactory<StudentDbContext>
{
    public StudentDbContext CreateDbContext(string[] args) => new(new DbContextOptionsBuilder<StudentDbContext>()
        .UseNpgsql(Environment.GetEnvironmentVariable("StudentConnection") ?? "Host=localhost;Database=student_lms;Username=postgres;Password=postgres").Options);
}

public static class DependencyInjection
{
    public static IServiceCollection AddStudentRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        var connection = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required.");
        services.AddDbContext<StudentDbContext>(options => options.UseNpgsql(connection));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}
