using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.Services.Models.Requests;

public class CreateEnrollmentRequest
{
    [Range(1, int.MaxValue)]
    public int StudentId { get; set; }

    [Range(1, int.MaxValue)]
    public int CourseId { get; set; }

    [Required]
    public DateTime EnrollDate { get; set; }

    [Required]
    [StringLength(20)]
    [RegularExpression("(?i)^(Active|Completed|Dropped|Pending)$")]
    public string Status { get; set; } = string.Empty;
}

public class UpdateEnrollmentRequest
{
    [Range(1, int.MaxValue)]
    public int StudentId { get; set; }

    [Range(1, int.MaxValue)]
    public int CourseId { get; set; }

    [Required]
    public DateTime EnrollDate { get; set; }

    [Required]
    [StringLength(20)]
    [RegularExpression("(?i)^(Active|Completed|Dropped|Pending)$")]
    public string Status { get; set; } = string.Empty;
}
