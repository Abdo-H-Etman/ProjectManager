using Application.Features.Users.DTOs;

namespace Application.Common.Interfaces;

public interface IIdentityService
{
    Task<(bool Success, Guid UserId, string FullName, string[] Errors)> CreateUserAsync(
        string email,
        string password,
        string firstName,
        string? lastName,
        CancellationToken cancellationToken = default);

    Task<(bool Success, Guid UserId, string Email, string FullName, IEnumerable<string> Roles, string[] Errors)> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);

    Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
