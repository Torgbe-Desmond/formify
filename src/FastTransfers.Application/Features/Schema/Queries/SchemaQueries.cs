using FastTransfers.Application.DTOs;
using FastTransfers.Domain.Entities;
using FastTransfers.Domain.Exceptions;
using FastTransfers.Domain.Interfaces;
using MediatR;

namespace FastTransfers.Application.Features.Schema.Queries;

// ── Query ─────────────────────────────────────────────────────────
public record GetSchemaQuery(Guid FolderId, Guid UserId)
    : IRequest<SchemaTemplateDto>;

// ── Handler ───────────────────────────────────────────────────────
public class GetSchemaQueryHandler
    : IRequestHandler<GetSchemaQuery, SchemaTemplateDto>
{
    private readonly ISchemaTemplateRepository _schemas;
    private readonly IFolderRepository _folders;

    public GetSchemaQueryHandler(
        ISchemaTemplateRepository schemas,
        IFolderRepository folders)
    {
        _schemas = schemas;
        _folders = folders;
    }

    public async Task<SchemaTemplateDto> Handle(
        GetSchemaQuery request,
        CancellationToken ct)
    {
        var folder = await _folders.GetByIdWithProjectAsync(request.FolderId, ct)
            ?? throw new NotFoundException(nameof(Folder), request.FolderId);

        folder.EnsureProjectOwnedBy(request.UserId);

        var schema = await _schemas.GetByFolderIdAsync(request.FolderId, ct)
            ?? throw new NotFoundException(
                $"No schema found for folder '{request.FolderId}'.");

        return new SchemaTemplateDto(
            schema.Id,
            schema.FolderId,
            schema.SchemaYaml,
            schema.TemplateHtml,
            schema.TemplateCss,
            schema.UpdatedAt);
    }
}