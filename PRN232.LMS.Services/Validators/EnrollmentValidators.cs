using FluentValidation;
using PRN232.LMS.Services.Models.Requests;

namespace PRN232.LMS.Services.Validators;

public class CreateEnrollmentRequestValidator : AbstractValidator<CreateEnrollmentRequest>
{
    public CreateEnrollmentRequestValidator()
    {
        RuleFor(request => request.StudentId)
            .GreaterThan(0);

        RuleFor(request => request.CourseId)
            .GreaterThan(0);

        RuleFor(request => request.EnrollDate)
            .NotEmpty();

        RuleFor(request => request.Status)
            .NotEmpty()
            .MaximumLength(20)
            .Must(BeValidStatus)
            .WithMessage("Status must be Active, Completed, Dropped, or Pending.");
    }

    private static bool BeValidStatus(string status)
    {
        string[] allowedStatuses = ["Active", "Completed", "Dropped", "Pending"];
        return allowedStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
    }
}

public class UpdateEnrollmentRequestValidator : AbstractValidator<UpdateEnrollmentRequest>
{
    public UpdateEnrollmentRequestValidator()
    {
        RuleFor(request => request.StudentId)
            .GreaterThan(0);

        RuleFor(request => request.CourseId)
            .GreaterThan(0);

        RuleFor(request => request.EnrollDate)
            .NotEmpty();

        RuleFor(request => request.Status)
            .NotEmpty()
            .MaximumLength(20)
            .Must(BeValidStatus)
            .WithMessage("Status must be Active, Completed, Dropped, or Pending.");
    }

    private static bool BeValidStatus(string status)
    {
        string[] allowedStatuses = ["Active", "Completed", "Dropped", "Pending"];
        return allowedStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
    }
}
