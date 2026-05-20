using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PRN232.LMS.API.Responses;

namespace PRN232.LMS.API.Filters;

// lớp này được sử dụng để thực hiện việc validate dữ liệu đầu vào của API. Nó sẽ kiểm tra ModelState để tìm lỗi và sử dụng FluentValidation để validate các đối tượng đầu vào. Nếu có lỗi, nó sẽ trả về một BadRequestObjectResult với thông tin lỗi chi tiết. Nếu không có lỗi, nó sẽ tiếp tục thực hiện action tiếp theo trong pipeline.
public class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        Dictionary<string, string[]> modelStateErrors = context.ModelState
            .Where(item => item.Value?.Errors.Count > 0)
            .ToDictionary(
                item => ToCamelCase(item.Key),
                item => item.Value!.Errors.Select(error => error.ErrorMessage).ToArray());

        List<ValidationFailure> failures = [];

        foreach (object? argument in context.ActionArguments.Values.Where(value => value is not null))
        {
            Type validatorType = typeof(IValidator<>).MakeGenericType(argument!.GetType());
            IEnumerable<IValidator> validators = context.HttpContext.RequestServices
                .GetServices(validatorType)
                .Cast<IValidator>();

            foreach (IValidator validator in validators)
            {
                ValidationContext<object> validationContext = new(argument);
                ValidationResult result = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);
                failures.AddRange(result.Errors);
            }
        }

        Dictionary<string, string[]> validationErrors = failures
            .GroupBy(failure => ToCamelCase(failure.PropertyName))
            .ToDictionary(
                group => group.Key,
                group => group.Select(failure => failure.ErrorMessage).Distinct().ToArray());

        Dictionary<string, string[]> errors = modelStateErrors
            .Concat(validationErrors)
            .GroupBy(item => item.Key)
            .ToDictionary(
                group => group.Key,
                group => group.SelectMany(item => item.Value).Distinct().ToArray());

        if (errors.Count > 0)
        {
            context.Result = new BadRequestObjectResult(ApiResponse<object?>.Fail("Validation failed", errors));
            return;
        }

        await next();
    }

    private static string ToCamelCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        string normalized = value.Contains('.', StringComparison.Ordinal)
            ? value.Split('.', StringSplitOptions.RemoveEmptyEntries).Last()
            : value;

        return char.ToLowerInvariant(normalized[0]) + normalized[1..];
    }
}
