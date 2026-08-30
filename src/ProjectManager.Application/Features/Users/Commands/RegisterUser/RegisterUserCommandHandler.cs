using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.Users.DTOs;
using FluentValidation.Results;
using MediatR;

namespace Application.Features.Users.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResponseDto>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterUserCommandHandler(
        IIdentityService identityService,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _identityService = identityService;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponseDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var (success, userId, fullName, errors) = await _identityService.CreateUserAsync(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            cancellationToken);

        if (!success)
        {
            var failures = errors.Select(e => new ValidationFailure("Registration", e));
            throw new ValidationException(failures);
        }

        var token = _jwtTokenGenerator.GenerateToken(userId, request.Email, fullName);

        return new AuthResponseDto
        {
            Id = userId,
            Email = request.Email,
            FullName = fullName,
            Token = token
        };
    }
}
