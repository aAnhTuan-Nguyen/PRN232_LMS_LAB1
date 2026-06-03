using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using PRN232.LMS.API.Filters;
using PRN232.LMS.API.Swagger;
using PRN232.LMS.Repositories;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers(options =>
    {
        options.Filters.Add<ApiExceptionFilter>();
        options.Filters.Add<ValidationFilter>();
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

builder.Services.AddRepositoryServices(builder.Configuration);  
builder.Services.AddApplicationServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
// Configure Swagger/OpenAPI documentation
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PRN232 LMS API",
        Version = "v1",
        Description = "RESTful API for PRN232 Lab 1 Learning Management System."
    });

    IncludeXmlCommentsIfExists(options, "PRN232.LMS.API.xml");
    IncludeXmlCommentsIfExists(options, "PRN232.LMS.Services.xml");
    options.OperationFilter<QueryParameterDescriptionsOperationFilter>();
});

var app = builder.Build();

await ApplyDatabaseMigrationsAsync(app);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // Map the Scalar API reference documentation at the "/docs" endpoint
    app.MapScalarApiReference("/docs", options =>
    {
        options
            .WithTitle("PRN232-Lab1")
            .WithOpenApiRoutePattern("/swagger/{documentName}/swagger.json");
    });
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "PRN232 LMS API v1");
    options.RoutePrefix = "swagger";
});

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

static void IncludeXmlCommentsIfExists(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options, string fileName)
{
    string xmlPath = Path.Combine(AppContext.BaseDirectory, fileName);

    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
}

static async Task ApplyDatabaseMigrationsAsync(WebApplication app)
{
    using IServiceScope scope = app.Services.CreateScope();
    LmsDbContext dbContext = scope.ServiceProvider.GetRequiredService<LmsDbContext>();
    ILoggerFactory loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
    ILogger logger = loggerFactory.CreateLogger("DatabaseMigration");

    const int maxAttempts = 10;
    TimeSpan delay = TimeSpan.FromSeconds(3);

    for (int attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully.");
            return;
        }
        catch (Exception ex) when (attempt < maxAttempts)
        {
            logger.LogWarning(
                ex,
                "Database migration attempt {Attempt}/{MaxAttempts} failed. Retrying in {DelaySeconds} seconds.",
                attempt,
                maxAttempts,
                delay.TotalSeconds);

            await Task.Delay(delay);
        }
    }

    await dbContext.Database.MigrateAsync();
}
