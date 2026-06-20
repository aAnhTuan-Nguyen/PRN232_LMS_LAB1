namespace PRN232.LMS.Services.Models.Business;

public class SemesterBusinessModel
{
    public int SemesterId { get; set; }

    public string SemesterName { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public IReadOnlyList<CourseBusinessModel>? Courses { get; set; }
}

public class SubjectBusinessModel
{
    public int SubjectId { get; set; }

    public string SubjectCode { get; set; } = string.Empty;

    public string SubjectName { get; set; } = string.Empty;

    public int Credit { get; set; }

    public IReadOnlyList<CourseBusinessModel>? Courses { get; set; }
}

public class CourseBusinessModel
{
    public int CourseId { get; set; }

    public string CourseName { get; set; } = string.Empty;

    public int SemesterId { get; set; }

    public SemesterBusinessModel? Semester { get; set; }

    public int SubjectId { get; set; }

    public SubjectBusinessModel? Subject { get; set; }

    public IReadOnlyList<EnrollmentBusinessModel>? Enrollments { get; set; }
}

public class StudentBusinessModel
{
    public int StudentId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    public IReadOnlyList<EnrollmentBusinessModel>? Enrollments { get; set; }
}

public class EnrollmentBusinessModel
{
    public int EnrollmentId { get; set; }

    public int StudentId { get; set; }

    public StudentBusinessModel? Student { get; set; }

    public int CourseId { get; set; }

    public CourseBusinessModel? Course { get; set; }

    public DateTime EnrollDate { get; set; }

    public string Status { get; set; } = string.Empty;
}
