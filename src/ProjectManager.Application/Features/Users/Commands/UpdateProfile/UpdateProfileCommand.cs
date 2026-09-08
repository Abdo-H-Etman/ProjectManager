using Application.Features.Users.DTOs;
using MediatR;

namespace Application.Features.Users.Commands.UpdateProfile;

public record UpdateProfileCommand(string? FirstName, string? LastName) : IRequest<UserDto>;
