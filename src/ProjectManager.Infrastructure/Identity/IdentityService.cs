using Application.Common.Interfaces;
using Application.Features.Users.DTOs;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<(bool Success, Guid UserId, string FullName, string[] Errors)> CreateUserAsync(
        string email,
        string password,
        string firstName,
        string? lastName,
        CancellationToken cancellationToken = default)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            return (false, Guid.Empty, string.Empty, ["User with this email already exists."]);
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return (false, Guid.Empty, string.Empty, result.Errors.Select(e => e.Description).ToArray());
        }

        return (true, user.Id, user.GetFullName(), Array.Empty<string>());
    }

    public async Task<(bool Success, Guid UserId, string Email, string FullName, IEnumerable<string> Roles, string[] Errors)> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || !user.IsActive || user.IsDeleted)
        {
            return (false, Guid.Empty, string.Empty, string.Empty, Enumerable.Empty<string>(), ["Invalid email or password."]);
        }

        var isValidPassword = await _userManager.CheckPasswordAsync(user, password);
        if (!isValidPassword)
        {
            return (false, Guid.Empty, string.Empty, string.Empty, Enumerable.Empty<string>(), ["Invalid email or password."]);
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        var roles = await _userManager.GetRolesAsync(user);

        return (true, user.Id, user.Email!, user.GetFullName(), roles, Array.Empty<string>());
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return null;
        }

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.GetFullName(),
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}
