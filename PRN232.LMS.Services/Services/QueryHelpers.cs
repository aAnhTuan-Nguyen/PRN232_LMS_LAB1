using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Services.Models.Common;

namespace PRN232.LMS.Services.Services;

internal static class QueryHelpers
{
    public static bool HasExpand(string? expand, string name)
    {
        return SplitCsv(expand).Any(value => string.Equals(value, name, StringComparison.OrdinalIgnoreCase));
    }

    public static async Task<PagedResult<object>> ToPagedResponseAsync<TEntity, TResponse>(
        IQueryable<TEntity> query,
        CollectionQueryParameters parameters,
        Func<TEntity, TResponse> map,
        CancellationToken cancellationToken)
        where TEntity : class
        where TResponse : class
    {
        int page = parameters.NormalizedPage;
        int size = parameters.NormalizedSize;
        int totalItems = await query.CountAsync(cancellationToken);
        List<TEntity> items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return new PagedResult<object>
        {
            Items = items.Select(item => SelectFields(map(item), parameters.Fields)).ToList(),
            Pagination = new PaginationMetadata
            {
                Page = page,
                PageSize = size,
                TotalItems = totalItems,
                TotalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)size)
            }
        };
    }

    public static IQueryable<T> ApplySort<T>(
        IQueryable<T> query,
        string? sort,
        IReadOnlyDictionary<string, LambdaExpression> allowedSorts,
        string defaultSort)
    {
        bool hasSort = false;

        foreach (string rawField in SplitCsv(sort))
        {
            bool descending = rawField.StartsWith("-", StringComparison.Ordinal);
            string field = descending ? rawField[1..] : rawField;

            if (!allowedSorts.TryGetValue(field, out LambdaExpression? expression))
            {
                continue;
            }

            query = ApplyOrder(query, expression, descending, hasSort);
            hasSort = true;
        }

        if (hasSort || !allowedSorts.TryGetValue(defaultSort, out LambdaExpression? defaultExpression))
        {
            return query;
        }

        return ApplyOrder(query, defaultExpression, descending: false, thenBy: false);
    }

    private static IQueryable<T> ApplyOrder<T>(
        IQueryable<T> source,
        LambdaExpression keySelector,
        bool descending,
        bool thenBy)
    {
        string methodName = thenBy
            ? descending ? nameof(Queryable.ThenByDescending) : nameof(Queryable.ThenBy)
            : descending ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);

        MethodInfo method = typeof(Queryable)
            .GetMethods()
            .Single(methodInfo => methodInfo.Name == methodName
                && methodInfo.GetParameters().Length == 2);

        MethodInfo genericMethod = method.MakeGenericMethod(typeof(T), keySelector.ReturnType);
        return (IQueryable<T>)genericMethod.Invoke(null, [source, keySelector])!;
    }

    private static object SelectFields<TResponse>(TResponse response, string? fields)
        where TResponse : class
    {
        string[] selectedFields = SplitCsv(fields).ToArray();

        if (selectedFields.Length == 0)
        {
            return response;
        }

        PropertyInfo[] properties = typeof(TResponse).GetProperties(BindingFlags.Instance | BindingFlags.Public);
        Dictionary<string, object?> result = new(StringComparer.OrdinalIgnoreCase);

        foreach (string selectedField in selectedFields)
        {
            PropertyInfo? property = properties.FirstOrDefault(candidate =>
                string.Equals(candidate.Name, selectedField, StringComparison.OrdinalIgnoreCase));

            if (property is null)
            {
                continue;
            }

            result[ToCamelCase(property.Name)] = property.GetValue(response);
        }

        return result;
    }

    private static IEnumerable<string> SplitCsv(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? []
            : value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    }

    private static string ToCamelCase(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? value
            : char.ToLowerInvariant(value[0]) + value[1..];
    }
}
