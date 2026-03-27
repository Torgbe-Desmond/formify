using System.Net.Mime;
using FastTransfers.Application.DTOs;
using FastTransfers.Application.Features.Files.Commands;
using FastTransfers.Application.Features.Files.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FastTransfers.API.Controllers;

[Route("api")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class FilesController : BaseController
{
    private readonly IMediator _mediator;

    public FilesController(IMediator mediator) => _mediator = mediator;

    /// <summary>List all files in a folder (no content — metadata only).</summary>
    [HttpGet("folders/{folderId:guid}/files")]
    [ProducesResponseType(typeof(IReadOnlyList<AppFileListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByFolder(Guid folderId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetFilesQuery(folderId, UserId), ct);
        return Ok(result);
    }

    /// <summary>Get a single file with its full rendered content and metadata.</summary>
    [HttpGet("files/{id:guid}")]
    [ProducesResponseType(typeof(FileDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var (dto, content) = await _mediator.Send(new GetFileQuery(id, UserId), ct);
        return Ok(new FileDetailResponse(dto, content));
    }

    /// <summary>Create a new file from rendered HTML and form metadata.</summary>
    [HttpPost("folders/{folderId:guid}/files")]
    [ProducesResponseType(typeof(AppFileDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(Guid folderId,
                                             [FromBody] CreateFileRequest request,
                                             CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateFileCommand(
            request.Name,
            folderId,
            UserId,
            request.RenderedHtml,
            request.Metadata), ct);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Update an existing file's content and metadata.</summary>
    [HttpPut("files/{id:guid}")]
    [ProducesResponseType(typeof(AppFileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(Guid id,
                                             [FromBody] UpdateFileRequest request,
                                             CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateFileCommand(
            id,
            UserId,
            request.Name,
            request.RenderedHtml,
            request.Metadata), ct);

        return Ok(result);
    }

    /// <summary>Delete a file and remove its content from storage.</summary>
    [HttpDelete("files/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteFileCommand(id, UserId), ct);
        return NoContent();
    }
}

// ── Request / Response models ─────────────────────────────────────
public record CreateFileRequest(
    string Name,
    string RenderedHtml,
    Dictionary<string, string> Metadata);

public record UpdateFileRequest(
    string Name,
    string RenderedHtml,
    Dictionary<string, string> Metadata);

public record FileDetailResponse(AppFileDto File, string Content);
