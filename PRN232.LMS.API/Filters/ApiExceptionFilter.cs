using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PRN232.LMS.API.Responses;
using PRN232.LMS.Services.Exceptions;

namespace PRN232.LMS.API.Filters;

// lớp này được sử dụng để xử lý các ngoại lệ (exception) xảy ra trong quá trình thực thi API. Nó sẽ bắt các ngoại lệ và trả về một ObjectResult phù hợp với loại ngoại lệ đó. Nếu là NotFoundException, nó sẽ trả về một NotFoundObjectResult với thông tin lỗi chi tiết. Nếu là ValidationException, nó sẽ trả về một BadRequestObjectResult với thông tin lỗi chi tiết. Nếu là bất kỳ loại ngoại lệ nào khác, nó sẽ ghi log lỗi và trả về một ObjectResult với thông tin lỗi chung.
public class ApiExceptionFilter(ILogger<ApiExceptionFilter> logger) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        context.Result = context.Exception switch
        {
            NotFoundException notFoundException => new NotFoundObjectResult(
                ApiResponse<object?>.Fail(notFoundException.Message)),

            ValidationException validationException => new BadRequestObjectResult(
                ApiResponse<object?>.Fail("Validation failed", BuildValidationErrors(validationException))),

            _ => BuildUnexpectedError(context.Exception)
        };

        context.ExceptionHandled = true;
    }

    private ObjectResult BuildUnexpectedError(Exception exception)
    {
        logger.LogError(exception, "Unhandled API exception.");

        return new ObjectResult(ApiResponse<object?>.Fail("An unexpected error occurred."))
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
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
