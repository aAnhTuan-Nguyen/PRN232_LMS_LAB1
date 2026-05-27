namespace PRN232.LMS.Services.Models.Common;

/// <summary>
/// Common query parameters for list endpoints.
/// </summary>
public class CollectionQueryParameters
{
    /// <summary>
    /// Keyword used to search text fields supported by the endpoint.
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Comma-separated fields used for sorting. Prefix a field with '-' for descending order.
    /// </summary>
    public string? Sort { get; set; }

    /// <summary>
    /// Page number to return. The first page is 1.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Number of items per page. Values outside 1-100 are normalized to 10.
    /// </summary>
    public int Size { get; set; } = 10;

    /// <summary>
    /// Comma-separated response fields to return.
    /// </summary>
    public string? Fields { get; set; }

    /// <summary>
    /// Comma-separated related resources to include in the response.
    /// </summary>
    public string? Expand { get; set; }

    /// <summary>
    /// Page number after normalization.
    /// </summary>
    public int NormalizedPage => Page < 1 ? 1 : Page;

    /// <summary>
    /// Page size after normalization.
    /// </summary>
    public int NormalizedSize => Size is < 1 or > 100 ? 10 : Size;
}
