using System.Net.Mime;
using FastTransfers.Application.DTOs;
using FastTransfers.Application.Features.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastTransfers.API.Controllers;

[ApiController]
[Route("api/auth")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) => _mediator = mediator;

    /// <summary>Register a new user account.</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command,
                                               CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(Register), result);
    }

    /// <summary>Login and receive a JWT token.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command,
                                            CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }
}
