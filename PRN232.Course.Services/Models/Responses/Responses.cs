namespace PRN232.Course.Services;

public sealed class SemesterResponse { public int SemesterId { get; set; } public string SemesterName { get; set; } = string.Empty; public DateTime StartDate { get; set; } public DateTime EndDate { get; set; } public IReadOnlyList<CourseSummary>? Courses { get; set; } }
public sealed class SubjectResponse { public int SubjectId { get; set; } public string SubjectCode { get; set; } = string.Empty; public string SubjectName { get; set; } = string.Empty; public int Credit { get; set; } public IReadOnlyList<CourseSummary>? Courses { get; set; } }
public sealed class CourseResponse { public int CourseId { get; set; } public string CourseName { get; set; } = string.Empty; public int SemesterId { get; set; } public SemesterSummary? Semester { get; set; } public int SubjectId { get; set; } public SubjectSummary? Subject { get; set; } public IReadOnlyList<EnrollmentResponse>? Enrollments { get; set; } }
public sealed class CourseSummary { public int CourseId { get; set; } public string CourseName { get; set; } = string.Empty; }
public sealed class SemesterSummary { public int SemesterId { get; set; } public string SemesterName { get; set; } = string.Empty; }
public sealed class SubjectSummary { public int SubjectId { get; set; } public string SubjectCode { get; set; } = string.Empty; public string SubjectName { get; set; } = string.Empty; }
public sealed class StudentSummary { public int StudentId { get; set; } public string FullName { get; set; } = string.Empty; public string Email { get; set; } = string.Empty; }
public sealed class EnrollmentResponse { public int EnrollmentId { get; set; } public int StudentId { get; set; } public StudentSummary? Student { get; set; } public int CourseId { get; set; } public CourseSummary? Course { get; set; } public DateTime EnrollDate { get; set; } public string Status { get; set; } = string.Empty; }
