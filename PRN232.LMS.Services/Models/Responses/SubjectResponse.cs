namespace PRN232.LMS.Services.Models.Responses;

public class SubjectResponse
{
    public int SubjectId { get; set; }

    public string SubjectCode { get; set; } = string.Empty;

    public string SubjectName { get; set; } = string.Empty;

    public int Credit { get; set; }

    public IReadOnlyList<CourseSummaryResponse>? Courses { get; set; }
}

public class SubjectSummaryResponse
{
    public int SubjectId { get; set; }

    public string SubjectCode { get; set; } = string.Empty;

    public string SubjectName { get; set; } = string.Empty;
}
