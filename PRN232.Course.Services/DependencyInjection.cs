using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace PRN232.Course.Services;

public static class DependencyInjection { public static IServiceCollection AddCourseApplicationServices(this IServiceCollection services) { services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly); services.AddScoped<ICourseLmsService, CourseLmsService>(); return services; } }
