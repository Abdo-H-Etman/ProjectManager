using Application.Common.Interfaces;
using Application.Features.Users.DTOs;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Users.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserDto>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;

    public GetCurrentUserQueryHandler(
        ICurrentUserService currentUserService,
        IIdentityService identityService)
    {
        _currentUserService = currentUserService;
        _identityService = identityService;
    }

    public async Task<UserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var user = await _identityService.GetUserByIdAsync(_currentUserService.UserId.Value, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException("User", _currentUserService.UserId.Value);
        }

        return user;
    }
}
