using System.Net.Mime;
using FastTransfers.Application.DTOs;
using FastTransfers.Application.Features.Projects.Commands;
using FastTransfers.Application.Features.Projects.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FastTransfers.API.Controllers;

[Route("api/projects")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class ProjectsController : BaseController
{
    private readonly IMediator _mediator;

    public ProjectsController(IMediator mediator) => _mediator = mediator;

    /// <summary>Get all projects for the authenticated user.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetProjectsQuery(UserId), ct);
        return Ok(result);
    }

    /// <summary>Get a single project by ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetProjectByIdQuery(id, UserId), ct);
        return Ok(result);
    }

    /// <summary>Create a new project.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] CreateProjectRequest request,
                                             CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateProjectCommand(request.Name, UserId), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Rename a project.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Rename(Guid id,
                                             [FromBody] RenameProjectRequest request,
                                             CancellationToken ct)
    {
        var result = await _mediator.Send(new RenameProjectCommand(id, request.Name, UserId), ct);
        return Ok(result);
    }

    /// <summary>Delete a project and all its folders and files.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteProjectCommand(id, UserId), ct);
        return NoContent();
    }
}

// ── Request models ────────────────────────────────────────────────
public record CreateProjectRequest(string Name);
public record RenameProjectRequest(string Name);
