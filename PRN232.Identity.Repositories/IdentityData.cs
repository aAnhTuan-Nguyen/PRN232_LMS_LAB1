using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PRN232.Identity.Repositories;

public sealed class User
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}

public sealed class RefreshToken
{
    public int RefreshTokenId { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByToken { get; set; }
    public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpiresAt;
}

public sealed class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");
            entity.HasKey(x => x.UserId);
            entity.Property(x => x.Username).HasMaxLength(50).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(255).IsRequired();
            entity.Property(x => x.Role).HasMaxLength(20).IsRequired();
            entity.HasIndex(x => x.Username).IsUnique();
        });
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshToken");
            entity.HasKey(x => x.RefreshTokenId);
            entity.Property(x => x.Token).HasMaxLength(256).IsRequired();
            entity.Property(x => x.ReplacedByToken).HasMaxLength(256);
            entity.Ignore(x => x.IsActive);
            entity.HasIndex(x => x.Token).IsUnique();
            entity.HasOne(x => x.User).WithMany(x => x.RefreshTokens).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<User>().HasData(
            new User { UserId = 1, Username = "admin", PasswordHash = "AQAAAAIAAYagAAAAEAARIjNEVWZ3iJmqu8zd7v9beM38/rDCq5QIQb9fxyMcTbTS7+2d/1D1jeksUJiSHA==", Role = "Admin" },
            new User { UserId = 2, Username = "student", PasswordHash = "AQAAAAIAAYagAAAAEBAhMkNUZXaHmKm6u9zd/g+Moi/X/8e6K5vowoVxX3V8sxKlZw6e8oWnp1y49EQJhw==", Role = "Student" });
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

internal sealed class GenericRepository<TEntity>(IdentityDbContext context) : IGenericRepository<TEntity> where TEntity : class
{
    public IQueryable<TEntity> Query() => context.Set<TEntity>();
    public async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => await context.Set<TEntity>().FindAsync([id], cancellationToken);
    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) => context.Set<TEntity>().AddAsync(entity, cancellationToken).AsTask();
    public void Update(TEntity entity) => context.Set<TEntity>().Update(entity);
    public void Remove(TEntity entity) => context.Set<TEntity>().Remove(entity);
}

public interface IUnitOfWork
{
    IGenericRepository<User> Users { get; }
    IGenericRepository<RefreshToken> RefreshTokens { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public sealed class UnitOfWork(IdentityDbContext context) : IUnitOfWork
{
    public IGenericRepository<User> Users { get; } = new GenericRepository<User>(context);
    public IGenericRepository<RefreshToken> RefreshTokens { get; } = new GenericRepository<RefreshToken>(context);
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => context.SaveChangesAsync(cancellationToken);
}

public sealed class IdentityDesignTimeFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseNpgsql(Environment.GetEnvironmentVariable("IdentityConnection") ?? "Host=localhost;Database=identity_lms;Username=postgres;Password=postgres")
            .Options;
        return new IdentityDbContext(options);
    }
}

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        var connection = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required.");
        services.AddDbContext<IdentityDbContext>(options => options.UseNpgsql(connection));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}
