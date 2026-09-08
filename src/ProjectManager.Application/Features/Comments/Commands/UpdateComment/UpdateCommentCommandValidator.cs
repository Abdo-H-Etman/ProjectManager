using FluentValidation;

namespace Application.Features.Comments.Commands.UpdateComment;

public class UpdateCommentCommandValidator : AbstractValidator<UpdateCommentCommand>
{
    public UpdateCommentCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Comment ID is required.");

        RuleFor(v => v.Content)
            .NotEmpty().WithMessage("Comment content is required.")
            .MaximumLength(2000).WithMessage("Comment content must not exceed 2000 characters.");
    }
}
