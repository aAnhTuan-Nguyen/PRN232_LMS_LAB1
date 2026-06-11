using FluentValidation;
using PRN232.LMS.Services.Models.Requests;

namespace PRN232.LMS.Services.Validators;

public class CreateSubjectRequestValidator : AbstractValidator<CreateSubjectRequest>
{
    public CreateSubjectRequestValidator()
    {
        RuleFor(request => request.SubjectCode)
            .NotEmpty()
            .MaximumLength(20)
            .Matches(@"^[A-Za-z]{2,5}[0-9]{3}$")
            .WithMessage("Subject code must follow the style PRN232.");

        RuleFor(request => request.SubjectName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.Credit)
            .InclusiveBetween(1, 10);
    }
}

public class UpdateSubjectRequestValidator : AbstractValidator<UpdateSubjectRequest>
{
    public UpdateSubjectRequestValidator()
    {
        RuleFor(request => request.SubjectCode)
            .NotEmpty()
            .MaximumLength(20)
            .Matches(@"^[A-Za-z]{2,5}[0-9]{3}$")
            .WithMessage("Subject code must follow the style PRN232.");

        RuleFor(request => request.SubjectName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.Credit)
            .InclusiveBetween(1, 10);
    }
}
