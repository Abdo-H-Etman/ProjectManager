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

    public async Task<IReadOnlyList<UserDto>> GetUsersAsync(string? searchTerm = null, CancellationToken cancellationToken = default)
    {
        var query = _userManager.Users
            .Where(u => !u.IsDeleted && u.IsActive);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(u =>
                u.FirstName.Contains(searchTerm) ||
                (u.LastName != null && u.LastName.Contains(searchTerm)) ||
                (u.Email != null && u.Email.Contains(searchTerm)));
        }

        var users = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(query, cancellationToken);

        return users.Select(u => new UserDto
        {
            Id = u.Id,
            Email = u.Email ?? string.Empty,
            FirstName = u.FirstName,
            LastName = u.LastName,
            FullName = u.GetFullName(),
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt
        }).ToList();
    }

    public async Task<(bool Success, string[] Errors)> UpdateProfileAsync(
        Guid userId,
        string? firstName,
        string? lastName,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null || user.IsDeleted)
        {
            return (false, ["User not found."]);
        }

        user.FirstName = string.IsNullOrWhiteSpace(firstName) ? user.FirstName : firstName;
        user.LastName = lastName ?? user.LastName;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return (false, result.Errors.Select(e => e.Description).ToArray());
        }

        return (true, Array.Empty<string>());
    }

    public async Task<(bool Success, string[] Errors)> ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null || user.IsDeleted)
        {
            return (false, ["User not found."]);
        }

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!result.Succeeded)
        {
            return (false, result.Errors.Select(e => e.Description).ToArray());
        }

        return (true, Array.Empty<string>());
    }
}
