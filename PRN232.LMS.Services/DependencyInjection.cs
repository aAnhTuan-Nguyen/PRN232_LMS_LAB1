using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PRN232.LMS.Services.Services;

namespace PRN232.LMS.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<ISemesterService, SemesterService>();
        services.AddScoped<ISubjectService, SubjectService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IEnrollmentService, EnrollmentService>();

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        return services.AddApplicationServices();
    }
}
