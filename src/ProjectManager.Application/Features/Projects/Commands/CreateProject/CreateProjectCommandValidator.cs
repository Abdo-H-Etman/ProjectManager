using FluentValidation;

namespace Application.Features.Projects.Commands.CreateProject;

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Project name is required.")
            .MaximumLength(150).WithMessage("Project name must not exceed 150 characters.");

        RuleFor(v => v.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");


        RuleFor(v => v.StartDate)
            .NotEmpty().WithMessage("StartDate is required.");

        RuleFor(v => v)
            .Must(v => !v.EndDate.HasValue || v.EndDate >= v.StartDate)
            .WithMessage("EndDate must be after or equal to StartDate.")
            .When(v => v.EndDate.HasValue);
    }
}
