using Application.Features.Users.DTOs;
using MediatR;

namespace Application.Features.Users.Queries.GetUsers;

public record GetUsersQuery(string? SearchTerm = null) : IRequest<IReadOnlyList<UserDto>>;
