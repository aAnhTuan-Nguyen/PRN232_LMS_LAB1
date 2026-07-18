namespace PRN232.Student.Services;

public sealed class StudentResponse
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public IReadOnlyList<EnrollmentSummary>? Enrollments { get; set; }
}
