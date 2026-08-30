using Application.Features.Users.DTOs;
using MediatR;

namespace Application.Features.Users.Commands.LoginUser;

public record LoginUserCommand : IRequest<AuthResponseDto>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
