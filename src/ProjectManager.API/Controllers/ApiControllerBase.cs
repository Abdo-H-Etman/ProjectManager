using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ProjectManager.API.Controllers;

/// <summary>Provides common behavior for API controllers.</summary>
[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _mediator;

    /// <summary>Gets the mediator used to dispatch application requests.</summary>
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}
