using Application.Features.Users.DTOs;
using MediatR;

namespace Application.Features.Users.Queries.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<UserDto>;
