using FluentValidation;
using TaskPriority = Domain.Enums.TaskPriority;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Features.Tasks.Commands.CreateSubtask;

public class CreateSubtaskCommandValidator : AbstractValidator<CreateSubtaskCommand>
{
    public CreateSubtaskCommandValidator()
    {
        RuleFor(v => v.ParentTaskId)
            .NotEmpty().WithMessage("Parent task ID is required.");

        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(v => v.Priority)
            .Must(p => string.IsNullOrWhiteSpace(p) || Enum.TryParse<TaskPriority>(p, true, out _))
            .WithMessage("Priority must be a valid task priority.");

        RuleFor(v => v.Status)
            .Must(s => string.IsNullOrWhiteSpace(s) || Enum.TryParse<TaskStatus>(s, true, out _))
            .WithMessage("Status must be a valid task status.");
    }
}
