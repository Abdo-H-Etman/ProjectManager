using Application.Features.Users.Commands.ChangePassword;
using Application.Features.Users.Commands.LoginUser;
using Application.Features.Users.Commands.RegisterUser;
using Application.Features.Users.Commands.UpdateProfile;
using Application.Features.Users.DTOs;
using Application.Features.Users.Queries.GetCurrentUser;
using Application.Features.Users.Queries.GetUsers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProjectManager.API.Controllers;

/// <summary>
/// Provides endpoints for managing user accounts.
/// </summary>
public class UsersController : ApiControllerBase
{
    /// <summary>Registers a new user account and returns a JWT.</summary>
    /// <param name="command">The new user's registration details.</param>
    /// <returns>The authenticated user details and token.</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterUserCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>Authenticates a user and returns a JWT.</summary>
    /// <param name="command">The user's login credentials.</param>
    /// <returns>The authenticated user details and token.</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginUserCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>Gets the currently authenticated user's profile.</summary>
    /// <returns>The current user's details.</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var result = await Mediator.Send(new GetCurrentUserQuery());
        return Ok(result);
    }

    /// <summary>Gets all active users for assignment and collaboration.</summary>
    /// <param name="searchTerm">Optional search query to filter by name or email.</param>
    /// <returns>A list of users.</returns>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(IReadOnlyList<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetUsers([FromQuery] string? searchTerm = null)
    {
        var result = await Mediator.Send(new GetUsersQuery(searchTerm));
        return Ok(result);
    }

    /// <summary>Updates the current user's profile details.</summary>
    /// <param name="command">Updated name information. All fields are optional.</param>
    /// <returns>The updated user profile.</returns>
    [HttpPut("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserDto>> UpdateProfile([FromBody] UpdateProfileCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>Changes the current user's password.</summary>
    /// <param name="command">Current and new password.</param>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        await Mediator.Send(command);
        return NoContent();
    }
}
