using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Users.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;

    public LogoutCommandHandler(ICurrentUserService currentUserService, IIdentityService identityService)
    {
        _currentUserService = currentUserService;
        _identityService = identityService;
    }

    public Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        return _identityService.LogoutAsync(_currentUserService.UserId.Value, cancellationToken);
    }
}