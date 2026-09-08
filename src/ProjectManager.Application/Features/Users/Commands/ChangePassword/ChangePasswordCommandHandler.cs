using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Users.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Unit>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;

    public ChangePasswordCommandHandler(
        ICurrentUserService currentUserService,
        IIdentityService identityService)
    {
        _currentUserService = currentUserService;
        _identityService = identityService;
    }

    public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var (success, errors) = await _identityService.ChangePasswordAsync(
            _currentUserService.UserId.Value,
            request.CurrentPassword,
            request.NewPassword,
            cancellationToken);

        if (!success)
        {
            throw new ValidationException(errors.Select(e => new FluentValidation.Results.ValidationFailure("Password", e)));
        }

        return Unit.Value;
    }
}
