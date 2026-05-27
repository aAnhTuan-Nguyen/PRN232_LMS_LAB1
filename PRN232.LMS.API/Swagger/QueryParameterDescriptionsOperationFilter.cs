using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PRN232.LMS.API.Swagger;

internal sealed class QueryParameterDescriptionsOperationFilter : IOperationFilter
{
    private static readonly ISet<string> HiddenParameters =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "NormalizedPage",
            "NormalizedSize"
        };

    private static readonly IReadOnlyDictionary<string, string> DisplayNames =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Search"] = "search",
            ["Sort"] = "sort",
            ["Page"] = "page",
            ["Size"] = "size",
            ["Fields"] = "fields",
            ["Expand"] = "expand"
        };

    private static readonly IReadOnlyDictionary<string, string> Descriptions =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = "Resource id from the route.",
            ["search"] = "Keyword used to search text fields supported by this endpoint. Example: active.",
            ["sort"] = "Comma-separated fields used for sorting. Prefix a field with '-' for descending order. Example: -enrollDate,status.",
            ["page"] = "Page number to return. The first page is 1.",
            ["size"] = "Number of items per page. Values outside 1-100 are normalized to 10.",
            ["fields"] = "Comma-separated response fields to return. Example: enrollmentId,status,enrollDate.",
            ["expand"] = "Comma-separated related resources to include. Examples: student, course, student,course."
        };

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters is null)
        {
            return;
        }

        for (int index = operation.Parameters.Count - 1; index >= 0; index--)
        {
            IOpenApiParameter parameter = operation.Parameters[index];

            if (string.IsNullOrWhiteSpace(parameter.Name))
            {
                continue;
            }

            if (HiddenParameters.Contains(parameter.Name))
            {
                operation.Parameters.RemoveAt(index);
                continue;
            }

            if (DisplayNames.TryGetValue(parameter.Name, out string? displayName)
                && parameter is OpenApiParameter openApiParameter)
            {
                openApiParameter.Name = displayName;
            }

            if (!Descriptions.TryGetValue(parameter.Name, out string? description))
            {
                continue;
            }

            parameter.Description = string.IsNullOrWhiteSpace(parameter.Description)
                ? description
                : parameter.Description;
        }
    }
}
