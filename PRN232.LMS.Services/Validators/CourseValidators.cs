using FluentValidation;
using PRN232.LMS.Services.Models.Requests;

namespace PRN232.LMS.Services.Validators;

public class CreateCourseRequestValidator : AbstractValidator<CreateCourseRequest>
{
    public CreateCourseRequestValidator()
    {
        RuleFor(request => request.CourseName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.SemesterId)
            .GreaterThan(0);

        RuleFor(request => request.SubjectId)
            .GreaterThan(0);
    }
}

public class UpdateCourseRequestValidator : AbstractValidator<UpdateCourseRequest>
{
    public UpdateCourseRequestValidator()
    {
        RuleFor(request => request.CourseName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.SemesterId)
            .GreaterThan(0);

        RuleFor(request => request.SubjectId)
            .GreaterThan(0);
    }
}
