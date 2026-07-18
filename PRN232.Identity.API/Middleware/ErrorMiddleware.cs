using PRN232.Identity.Services;

namespace PRN232.Identity.API;

public sealed class ErrorMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (UnauthorizedException)
        {
            await Write(context, StatusCodes.Status401Unauthorized, "Unauthorized");
        }
        catch (Exception exception)
        {
            var status = exception is BadHttpRequestException
                ? StatusCodes.Status400BadRequest
                : StatusCodes.Status500InternalServerError;
            var message = status == StatusCodes.Status500InternalServerError
                ? "An unexpected error occurred."
                : exception.Message;

            await Write(context, status, message);
        }
    }

    private static Task Write(HttpContext context, int status, string message)
    {
        context.Response.StatusCode = status;
        return context.Response.WriteAsJsonAsync(ApiResponse<object?>.Fail(message));
    }
}
