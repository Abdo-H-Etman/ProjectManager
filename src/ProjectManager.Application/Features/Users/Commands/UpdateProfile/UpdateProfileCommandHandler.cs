using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.Users.DTOs;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Users.Commands.UpdateProfile;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, UserDto>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;

    public UpdateProfileCommandHandler(
        ICurrentUserService currentUserService,
        IIdentityService identityService)
    {
        _currentUserService = currentUserService;
        _identityService = identityService;
    }

    public async Task<UserDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var (success, errors) = await _identityService.UpdateProfileAsync(
            _currentUserService.UserId.Value,
            request.FirstName ?? string.Empty,
            request.LastName,
            cancellationToken);

        if (!success)
        {
            throw new ValidationException(errors.Select(e => new FluentValidation.Results.ValidationFailure("Profile", e)));
        }

        var user = await _identityService.GetUserByIdAsync(_currentUserService.UserId.Value, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException("User", _currentUserService.UserId.Value);
        }

        return user;
    }
}
