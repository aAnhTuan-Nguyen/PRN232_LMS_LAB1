using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.Services.Models.Requests;

public class CreateSubjectRequest
{
    [Required]
    [StringLength(20)]
    [RegularExpression(@"^[A-Za-z]{2,5}[0-9]{3}$")]
    public string SubjectCode { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string SubjectName { get; set; } = string.Empty;

    [Range(1, 10)]
    public int Credit { get; set; }
}

public class UpdateSubjectRequest
{
    [Required]
    [StringLength(20)]
    [RegularExpression(@"^[A-Za-z]{2,5}[0-9]{3}$")]
    public string SubjectCode { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string SubjectName { get; set; } = string.Empty;

    [Range(1, 10)]
    public int Credit { get; set; }
}
