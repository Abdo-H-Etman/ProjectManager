using Application.Common.Interfaces;
using Application.Features.Users.DTOs;
using MediatR;

namespace Application.Features.Users.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RefreshTokenCommandHandler(IIdentityService identityService, IJwtTokenGenerator jwtTokenGenerator)
    {
        _identityService = identityService;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.RefreshSessionAsync(request.RefreshToken, cancellationToken);
        if (!result.Success)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");
        }

        var securityStamp = await _identityService.GetSecurityStampAsync(result.UserId, cancellationToken);

        return new AuthResponseDto
        {
            Id = result.UserId,
            Email = result.Email,
            FullName = result.FullName,
            Token = _jwtTokenGenerator.GenerateToken(result.UserId, result.Email, result.FullName, result.Roles, securityStamp),
            RefreshToken = result.RefreshToken,
            AccessTokenExpiresAt = _jwtTokenGenerator.GetAccessTokenExpiration()
        };
    }
}