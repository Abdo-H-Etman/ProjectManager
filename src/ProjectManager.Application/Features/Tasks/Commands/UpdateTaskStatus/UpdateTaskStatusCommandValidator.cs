using FluentValidation;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Features.Tasks.Commands.UpdateTaskStatus;

public class UpdateTaskStatusCommandValidator : AbstractValidator<UpdateTaskStatusCommand>
{
    public UpdateTaskStatusCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Task ID is required.");

        RuleFor(v => v.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(status => Enum.TryParse<TaskStatus>(status, true, out _))
            .WithMessage("Status must be a valid task status (Pending, InProgress, Review, Blocked, Completed, Cancelled).");
    }
}
