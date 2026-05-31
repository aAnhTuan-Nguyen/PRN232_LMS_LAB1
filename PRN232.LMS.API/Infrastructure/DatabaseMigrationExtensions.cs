using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;

namespace PRN232.LMS.API.Infrastructure;

public static class DatabaseMigrationExtensions
{
    public static async Task MigrateDatabaseAsync(this WebApplication app)
    {
        const int maxAttempts = 10;
        TimeSpan retryDelay = TimeSpan.FromSeconds(3);

        ILogger logger = app.Services
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("DatabaseMigration");

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
                LmsDbContext dbContext = scope.ServiceProvider.GetRequiredService<LmsDbContext>();

                logger.LogInformation("Applying database migrations.");
                await dbContext.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied successfully.");

                return;
            }
            catch (Exception exception) when (attempt < maxAttempts)
            {
                logger.LogWarning(
                    exception,
                    "Database migration attempt {Attempt}/{MaxAttempts} failed. Retrying in {RetryDelaySeconds} seconds.",
                    attempt,
                    maxAttempts,
                    retryDelay.TotalSeconds);

                await Task.Delay(retryDelay);
            }
        }
    }
}
