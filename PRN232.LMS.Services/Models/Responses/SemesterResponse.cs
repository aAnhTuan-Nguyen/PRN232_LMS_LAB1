namespace PRN232.LMS.Services.Models.Responses;

public class SemesterResponse
{
    public int SemesterId { get; set; }

    public string SemesterName { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public IReadOnlyList<CourseSummaryResponse>? Courses { get; set; }
}

public class SemesterSummaryResponse
{
    public int SemesterId { get; set; }

    public string SemesterName { get; set; } = string.Empty;
}
