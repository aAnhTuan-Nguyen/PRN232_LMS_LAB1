using FluentValidation;

namespace PRN232.Course.Services;

public sealed class SemesterValidator : AbstractValidator<SemesterRequest> { public SemesterValidator() { RuleFor(x => x.SemesterName).NotEmpty().MaximumLength(100); RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate); } }
public sealed class SubjectValidator : AbstractValidator<SubjectRequest> { public SubjectValidator() { RuleFor(x => x.SubjectCode).Matches("^[A-Za-z]{2,5}[0-9]{3}$").MaximumLength(20); RuleFor(x => x.SubjectName).NotEmpty().MaximumLength(100); RuleFor(x => x.Credit).InclusiveBetween(1, 10); } }
public sealed class CourseValidator : AbstractValidator<CourseRequest> { public CourseValidator() { RuleFor(x => x.CourseName).NotEmpty().MaximumLength(100); RuleFor(x => x.SemesterId).GreaterThan(0); RuleFor(x => x.SubjectId).GreaterThan(0); } }
public sealed class EnrollmentValidator : AbstractValidator<EnrollmentRequest> { public EnrollmentValidator() { RuleFor(x => x.StudentId).GreaterThan(0); RuleFor(x => x.CourseId).GreaterThan(0); RuleFor(x => x.EnrollDate).NotEmpty(); RuleFor(x => x.Status).Must(x => new[] { "Active", "Completed", "Dropped", "Pending" }.Contains(x, StringComparer.OrdinalIgnoreCase)).WithMessage("Status must be Active, Completed, Dropped, or Pending."); } }
