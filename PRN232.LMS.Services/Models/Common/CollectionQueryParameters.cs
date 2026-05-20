namespace PRN232.LMS.Services.Models.Common;

public class CollectionQueryParameters
{
    public string? Search { get; set; }

    public string? Sort { get; set; }

    public int Page { get; set; } = 1;

    public int Size { get; set; } = 10;

    public string? Fields { get; set; }

    public string? Expand { get; set; }

    public int NormalizedPage => Page < 1 ? 1 : Page;

    public int NormalizedSize => Size is < 1 or > 100 ? 10 : Size;
}
