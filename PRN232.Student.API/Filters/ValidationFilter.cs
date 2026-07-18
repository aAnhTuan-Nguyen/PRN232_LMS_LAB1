using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PRN232.Student.API;

public sealed class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

        foreach (var value in context.ActionArguments.Values.Where(value => value is not null))
        {
            var validatorType = typeof(IValidator<>).MakeGenericType(value!.GetType());
            var validators = context.HttpContext.RequestServices.GetServices(validatorType).Cast<IValidator>();

            foreach (var validator in validators)
            {
                var result = await validator.ValidateAsync(
                    new ValidationContext<object>(value),
                    context.HttpContext.RequestAborted);

                foreach (var group in result.Errors.GroupBy(error => error.PropertyName))
                {
                    errors[group.Key] = group.Select(error => error.ErrorMessage).ToArray();
                }
            }
        }

        if (errors.Count > 0)
        {
            context.Result = new BadRequestObjectResult(ApiResponse<object?>.Fail("Validation failed", errors));
            return;
        }

        await next();
    }
}
