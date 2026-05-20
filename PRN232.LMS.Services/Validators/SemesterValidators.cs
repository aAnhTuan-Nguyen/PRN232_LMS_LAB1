using FluentValidation;
using PRN232.LMS.Services.Models.Requests;

namespace PRN232.LMS.Services.Validators;

public class CreateSemesterRequestValidator : AbstractValidator<CreateSemesterRequest>
{
    public CreateSemesterRequestValidator()
    {
        RuleFor(request => request.SemesterName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.StartDate)
            .LessThan(request => request.EndDate)
            .WithMessage("Start date must be before end date.");

        RuleFor(request => request.EndDate)
            .NotEmpty();
    }
}

public class UpdateSemesterRequestValidator : AbstractValidator<UpdateSemesterRequest>
{
    public UpdateSemesterRequestValidator()
    {
        RuleFor(request => request.SemesterName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.StartDate)
            .LessThan(request => request.EndDate)
            .WithMessage("Start date must be before end date.");

        RuleFor(request => request.EndDate)
            .NotEmpty();
    }
}
