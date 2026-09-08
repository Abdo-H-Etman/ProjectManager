using FluentValidation;

namespace Application.Features.Tasks.Commands.AssignTask;

public class AssignTaskCommandValidator : AbstractValidator<AssignTaskCommand>
{
    public AssignTaskCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Task ID is required.");
    }
}
