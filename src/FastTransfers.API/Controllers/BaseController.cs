using System.Net.Mime;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastTransfers.API.Controllers;

[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
[Authorize]
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// Extracts the authenticated user's ID from the JWT sub claim.
    /// </summary>
    protected Guid UserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException("User ID not found in token."));
}
