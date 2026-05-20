namespace PRN232.LMS.Services.Models.Responses;

public class EnrollmentResponse
{
    public int EnrollmentId { get; set; }

    public int StudentId { get; set; }

    public StudentSummaryResponse? Student { get; set; }

    public int CourseId { get; set; }

    public CourseSummaryResponse? Course { get; set; }

    public DateTime EnrollDate { get; set; }

    public string Status { get; set; } = string.Empty;
}

public class EnrollmentSummaryResponse
{
    public int EnrollmentId { get; set; }

    public DateTime EnrollDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public CourseSummaryResponse? Course { get; set; }
}
