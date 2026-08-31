using FluentValidation;

namespace Application.Features.Tasks.Commands.UpdateTask;

public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Task title is required.")
            .MaximumLength(200).WithMessage("Task title must not exceed 200 characters.");

        RuleFor(v => v.Description)
            .MaximumLength(4000).WithMessage("Description must not exceed 4000 characters.");

        RuleFor(v => v.EstimatedHours)
            .GreaterThanOrEqualTo(0).WithMessage("Estimated hours must be greater than or equal to 0.")
            .When(v => v.EstimatedHours.HasValue);

        RuleFor(v => v.ActualHours)
            .GreaterThanOrEqualTo(0).WithMessage("Actual hours must be greater than or equal to 0.")
            .When(v => v.ActualHours.HasValue);

        RuleFor(v => v)
            .Must(v => !v.DueDate.HasValue || !v.StartDate.HasValue || v.DueDate >= v.StartDate)
            .WithMessage("DueDate must be after or equal to StartDate.")
            .When(v => v.DueDate.HasValue && v.StartDate.HasValue);
    }
}
