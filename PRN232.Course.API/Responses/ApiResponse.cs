namespace PRN232.Course.API;

public sealed class ApiResponse<T>
{
    public bool Success { get; init; }

    public string Message { get; init; } = string.Empty;

    public T? Data { get; init; }

    public object? Pagination { get; init; }

    public IReadOnlyDictionary<string, string[]>? Errors { get; init; }

    public static ApiResponse<T> Ok(T? data, string message, object? pagination = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Pagination = pagination
        };
    }

    public static ApiResponse<T> Fail(
        string message,
        IReadOnlyDictionary<string, string[]>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}
