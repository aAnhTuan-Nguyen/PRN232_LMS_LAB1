using FluentValidation;

namespace PRN232.Student.Services;

public sealed class StudentRequestValidator : AbstractValidator<CreateStudentRequest>
{
    public StudentRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(100);
        RuleFor(x => x.DateOfBirth).LessThan(DateTime.UtcNow);
    }
}
