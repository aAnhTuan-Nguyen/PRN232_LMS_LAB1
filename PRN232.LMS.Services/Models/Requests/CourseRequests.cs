using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.Services.Models.Requests;

public class CreateCourseRequest
{
    [Required]
    [StringLength(100)]
    public string CourseName { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int SemesterId { get; set; }

    [Range(1, int.MaxValue)]
    public int SubjectId { get; set; }
}

public class UpdateCourseRequest
{
    [Required]
    [StringLength(100)]
    public string CourseName { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int SemesterId { get; set; }

    [Range(1, int.MaxValue)]
    public int SubjectId { get; set; }
}
