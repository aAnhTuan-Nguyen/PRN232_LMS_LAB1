using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PRN232.Identity.API;

public sealed class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var failures = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

        foreach (var argument in context.ActionArguments.Values.Where(value => value is not null))
        {
            var validatorType = typeof(IValidator<>).MakeGenericType(argument!.GetType());
            var validators = context.HttpContext.RequestServices.GetServices(validatorType).Cast<IValidator>();

            foreach (var validator in validators)
            {
                var result = await validator.ValidateAsync(
                    new ValidationContext<object>(argument),
                    context.HttpContext.RequestAborted);

                foreach (var group in result.Errors.GroupBy(error => error.PropertyName))
                {
                    failures[group.Key] = group.Select(error => error.ErrorMessage).ToArray();
                }
            }
        }

        if (failures.Count > 0)
        {
            context.Result = new BadRequestObjectResult(
                ApiResponse<object?>.Fail("Validation failed", failures));
            return;
        }

        await next();
    }
}
