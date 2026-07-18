using PRN232.Course.Services;

namespace PRN232.Course.API;

public sealed class ErrorMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException exception)
        {
            await Write(context, StatusCodes.Status404NotFound, exception.Message);
        }
        catch (BadRequestException exception)
        {
            await Write(context, StatusCodes.Status400BadRequest, exception.Message);
        }
        catch (Exception)
        {
            await Write(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }

    private static Task Write(HttpContext context, int status, string message)
    {
        context.Response.StatusCode = status;
        return context.Response.WriteAsJsonAsync(ApiResponse<object?>.Fail(message));
    }
}
