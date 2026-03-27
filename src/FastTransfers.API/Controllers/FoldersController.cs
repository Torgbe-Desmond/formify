using System.Net.Mime;
using FastTransfers.Application.DTOs;
using FastTransfers.Application.Features.Folders.Commands;
using FastTransfers.Application.Features.Folders.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FastTransfers.API.Controllers;

[Route("api")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class FoldersController : BaseController
{
    private readonly IMediator _mediator;

    public FoldersController(IMediator mediator) => _mediator = mediator;

    /// <summary>Get all folders in a project.</summary>
    [HttpGet("projects/{projectId:guid}/folders")]
    [ProducesResponseType(typeof(IReadOnlyList<FolderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByProject(Guid projectId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetFoldersQuery(projectId, UserId), ct);
        return Ok(result);
    }

    /// <summary>Create a folder inside a project.</summary>
    [HttpPost("projects/{projectId:guid}/folders")]
    [ProducesResponseType(typeof(FolderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(Guid projectId,
                                             [FromBody] FolderNameRequest request,
                                             CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateFolderCommand(request.Name, projectId, UserId), ct);
        return CreatedAtAction(nameof(GetByProject), new { projectId }, result);
    }

    /// <summary>Rename a folder.</summary>
    [HttpPut("folders/{id:guid}")]
    [ProducesResponseType(typeof(FolderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Rename(Guid id,
                                             [FromBody] FolderNameRequest request,
                                             CancellationToken ct)
    {
        var result = await _mediator.Send(new RenameFolderCommand(id, request.Name, UserId), ct);
        return Ok(result);
    }

    /// <summary>Delete a folder and all its files.</summary>
    [HttpDelete("folders/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteFolderCommand(id, UserId), ct);
        return NoContent();
    }
}

public record FolderNameRequest(string Name);
