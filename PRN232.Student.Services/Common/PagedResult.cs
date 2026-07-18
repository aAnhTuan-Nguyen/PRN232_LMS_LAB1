namespace PRN232.Student.Services;

public sealed class PagedResult
{
    public IReadOnlyList<object> Items { get; init; } = [];
    public PaginationMetadata Pagination { get; init; } = new();
}
