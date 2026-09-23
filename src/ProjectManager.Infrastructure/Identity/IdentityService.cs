using Application.Common.Interfaces;
using Application.Features.Users.DTOs;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _dbContext;

    public IdentityService(UserManager<ApplicationUser> userManager, AppDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
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

    public async Task<string> CreateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new InvalidOperationException("User not found.");
        return await CreateRefreshTokenAsync(user, cancellationToken);
    }

    public async Task<(bool Success, Guid UserId, string Email, string FullName, IEnumerable<string> Roles, string RefreshToken, string[] Errors)> RefreshSessionAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var storedToken = await _dbContext.RefreshTokens
            .Include(token => token.User)
            .SingleOrDefaultAsync(token => token.TokenHash == HashToken(refreshToken), cancellationToken);

        if (storedToken == null || storedToken.RevokedAt.HasValue || storedToken.ExpiresAt <= DateTime.UtcNow ||
            !storedToken.User.IsActive || storedToken.User.IsDeleted)
        {
            return (false, Guid.Empty, string.Empty, string.Empty, Enumerable.Empty<string>(), string.Empty, ["Invalid or expired refresh token."]);
        }

        storedToken.RevokedAt = DateTime.UtcNow;
        var roles = await _userManager.GetRolesAsync(storedToken.User);
        var newRefreshToken = await CreateRefreshTokenAsync(storedToken.User, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return (true, storedToken.User.Id, storedToken.User.Email!, storedToken.User.GetFullName(), roles, newRefreshToken, Array.Empty<string>());
    }

    public async Task LogoutAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user != null)
        {
            var refreshTokens = await _dbContext.RefreshTokens
                .Where(token => token.UserId == user.Id && !token.RevokedAt.HasValue)
                .ToListAsync(cancellationToken);

            foreach (var token in refreshTokens)
            {
                token.RevokedAt = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            await _userManager.UpdateSecurityStampAsync(user);
        }
    }

    public async Task<string?> GetSecurityStampAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user == null ? null : await _userManager.GetSecurityStampAsync(user);
    }

    private async Task<string> CreateRefreshTokenAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        _dbContext.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = HashToken(rawToken),
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        });
        await _dbContext.SaveChangesAsync(cancellationToken);
        return rawToken;
    }

    private static string HashToken(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

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
