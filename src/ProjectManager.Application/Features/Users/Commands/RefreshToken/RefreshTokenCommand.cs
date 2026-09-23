using Application.Features.Users.DTOs;
using MediatR;

namespace Application.Features.Users.Commands.RefreshToken;

public record RefreshTokenCommand : IRequest<AuthResponseDto>
{
    public string RefreshToken { get; init; } = string.Empty;
}