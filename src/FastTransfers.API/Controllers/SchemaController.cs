using System.Net.Mime;
using FastTransfers.Application.DTOs;
using FastTransfers.Application.Features.Schema.Commands;
using FastTransfers.Application.Features.Schema.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FastTransfers.API.Controllers;

[Route("api/folders/{folderId:guid}/schema")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class SchemaController : BaseController
{
    private readonly IMediator _mediator;

    public SchemaController(IMediator mediator) => _mediator = mediator;

    /// <summary>Get the schema template for a folder.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(SchemaTemplateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid folderId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetSchemaQuery(folderId, UserId), ct);
        return Ok(result);
    }

    /// <summary>Create or update the schema template for a folder.</summary>
    [HttpPut]
    [ProducesResponseType(typeof(SchemaTemplateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Upsert(Guid folderId,
                                             [FromBody] UpsertSchemaRequest request,
                                             CancellationToken ct)
    {
        var result = await _mediator.Send(new UpsertSchemaCommand(
            folderId,
            UserId,
            request.SchemaYaml,
            request.TemplateHtml,
            request.TemplateCss), ct);

        return Ok(result);
    }
}

public record UpsertSchemaRequest(
    string SchemaYaml,
    string TemplateHtml,
    string TemplateCss);
