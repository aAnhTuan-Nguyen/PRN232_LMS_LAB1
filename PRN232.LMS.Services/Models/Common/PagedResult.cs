namespace PRN232.LMS.Services.Models.Common;

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = [];

    public PaginationMetadata Pagination { get; set; } = new();
}
