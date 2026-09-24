using Application.Common.Exceptions;
using Application.Common.Interfaces;

namespace Application.Common.Authorization;

public static class AuthorizationRules
{
    public static Guid RequireAuthenticatedUser(ICurrentUserService currentUserService)
    {
        if (!currentUserService.UserId.HasValue || !currentUserService.IsAuthenticated)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        return currentUserService.UserId.Value;
    }

    public static void RequireOwner(ICurrentUserService currentUserService, Guid ownerId, string message = "You are not authorized to modify this resource.")
    {
        var userId = RequireAuthenticatedUser(currentUserService);
        if (ownerId != userId)
        {
            throw new ForbiddenAccessException(message);
        }
    }

    public static void RequireOwnerOrAdmin(
        ICurrentUserService currentUserService,
        Guid ownerId,
        string message = "You are not authorized to modify this resource.")
    {
        var userId = RequireAuthenticatedUser(currentUserService);
        if (ownerId != userId && !currentUserService.IsInRole(AuthorizationRoles.Admin))
        {
            throw new ForbiddenAccessException(message);
        }
    }
}