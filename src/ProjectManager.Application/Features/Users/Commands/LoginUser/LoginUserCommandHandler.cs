using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.Users.DTOs;
using FluentValidation.Results;
using MediatR;

namespace Application.Features.Users.Commands.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthResponseDto>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginUserCommandHandler(
        IIdentityService identityService,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _identityService = identityService;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponseDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var (success, userId, email, fullName, roles, errors) = await _identityService.AuthenticateAsync(
            request.Email,
            request.Password,
            cancellationToken);

        if (!success)
        {
            var failures = errors.Select(e => new ValidationFailure("Authentication", e));
            throw new ValidationException(failures);
        }

        var token = _jwtTokenGenerator.GenerateToken(userId, email, fullName, roles);

        return new AuthResponseDto
        {
            Id = userId,
            Email = email,
            FullName = fullName,
            Token = token
        };
    }
}
