namespace PRN232.Course.Services;

public sealed class SemesterRequest { public string SemesterName { get; set; } = string.Empty; public DateTime StartDate { get; set; } public DateTime EndDate { get; set; } }
public sealed class SubjectRequest { public string SubjectCode { get; set; } = string.Empty; public string SubjectName { get; set; } = string.Empty; public int Credit { get; set; } }
public sealed class CourseRequest { public string CourseName { get; set; } = string.Empty; public int SemesterId { get; set; } public int SubjectId { get; set; } }
public sealed class EnrollmentRequest { public int StudentId { get; set; } public int CourseId { get; set; } public DateTime EnrollDate { get; set; } public string Status { get; set; } = string.Empty; }
