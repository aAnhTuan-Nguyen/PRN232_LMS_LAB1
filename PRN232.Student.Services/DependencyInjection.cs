using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace PRN232.Student.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddStudentApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<IStudentService, StudentService>();

        return services;
    }
}
