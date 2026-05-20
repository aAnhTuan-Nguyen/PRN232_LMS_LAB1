namespace PRN232.LMS.Services.Models.Responses;

public class CourseResponse
{
    public int CourseId { get; set; }

    public string CourseName { get; set; } = string.Empty;

    public int SemesterId { get; set; }

    public SemesterSummaryResponse? Semester { get; set; }

    public int SubjectId { get; set; }

    public SubjectSummaryResponse? Subject { get; set; }

    public IReadOnlyList<EnrollmentSummaryResponse>? Enrollments { get; set; }
}

public class CourseSummaryResponse
{
    public int CourseId { get; set; }

    public string CourseName { get; set; } = string.Empty;

    public SemesterSummaryResponse? Semester { get; set; }

    public SubjectSummaryResponse? Subject { get; set; }
}
