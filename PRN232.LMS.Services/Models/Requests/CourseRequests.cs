namespace PRN232.LMS.Services.Models.Requests;

public class CreateCourseRequest
{
    public string CourseName { get; set; } = string.Empty;

    public int SemesterId { get; set; }

    public int SubjectId { get; set; }
}

public class UpdateCourseRequest
{
    public string CourseName { get; set; } = string.Empty;

    public int SemesterId { get; set; }

    public int SubjectId { get; set; }
}
