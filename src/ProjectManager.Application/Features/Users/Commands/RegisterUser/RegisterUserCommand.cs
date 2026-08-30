using Application.Features.Users.DTOs;
using MediatR;

namespace Application.Features.Users.Commands.RegisterUser;

public record RegisterUserCommand : IRequest<AuthResponseDto>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string? LastName { get; init; }
}
