using FluentValidation;
using PRN232.LMS.API.Responses;
using PRN232.LMS.Services.Exceptions;

namespace PRN232.LMS.API.Middleware;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
        {
            logger.LogError(exception, "Unhandled exception after response started.");
            throw exception;
        }

        (int statusCode, string message, object? errors) = exception switch
        {
            ValidationException validationException => (
                StatusCodes.Status400BadRequest,
                "Validation failed",
                BuildValidationErrors(validationException)),

            NotFoundException notFoundException => (
                StatusCodes.Status404NotFound,
                notFoundException.Message,
                null),

            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                null),

            BadHttpRequestException badHttpRequestException => (
                StatusCodes.Status400BadRequest,
                badHttpRequestException.Message,
                null),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal server error",
                null)
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled API exception.");
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(ApiResponse<object?>.Fail(message, errors));
    }

    private static Dictionary<string, string[]> BuildValidationErrors(ValidationException exception)
    {
        return exception.Errors
            .GroupBy(error => ToCamelCase(error.PropertyName))
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage).Distinct().ToArray());
    }

    private static string ToCamelCase(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? value
            : char.ToLowerInvariant(value[0]) + value[1..];
    }
}
