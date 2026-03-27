using FastTransfers.Application.DTOs;
using FastTransfers.Domain.Entities;
using FastTransfers.Domain.Exceptions;
using FastTransfers.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace FastTransfers.Application.Features.Schema.Commands;

// ── Command ────────────────────────────────────────────────────────
public record UpsertSchemaCommand(
    Guid FolderId,
    Guid UserId,
    string SchemaYaml,
    string TemplateHtml,
    string TemplateCss) : IRequest<SchemaTemplateDto>;

// ── Validator ─────────────────────────────────────────────────────
public class UpsertSchemaCommandValidator : AbstractValidator<UpsertSchemaCommand>
{
    public UpsertSchemaCommandValidator()
    {
        RuleFor(x => x.FolderId).NotEmpty();
        RuleFor(x => x.SchemaYaml).NotEmpty();
    }
}

// ── Handler ───────────────────────────────────────────────────────
public class UpsertSchemaCommandHandler
    : IRequestHandler<UpsertSchemaCommand, SchemaTemplateDto>
{
    private readonly ISchemaTemplateRepository _schemas;
    private readonly IFolderRepository _folders;
    private readonly IUnitOfWork _uow;

    public UpsertSchemaCommandHandler(
        ISchemaTemplateRepository schemas,
        IFolderRepository folders,
        IUnitOfWork uow)
    {
        _schemas = schemas;
        _folders = folders;
        _uow = uow;
    }

    public async Task<SchemaTemplateDto> Handle(
        UpsertSchemaCommand request,
        CancellationToken ct)
    {
        var folder = await _folders.GetByIdWithProjectAsync(request.FolderId, ct)
            ?? throw new NotFoundException(nameof(Folder), request.FolderId);

        folder.EnsureProjectOwnedBy(request.UserId);

        var existing = await _schemas.GetByFolderIdAsync(request.FolderId, ct);

        SchemaTemplate schema;

        if (existing is null)
        {
            schema = SchemaTemplate.Create(
                request.FolderId,
                request.SchemaYaml,
                request.TemplateHtml,
                request.TemplateCss);

            await _schemas.AddAsync(schema, ct);
        }
        else
        {
            existing.Update(
                request.SchemaYaml,
                request.TemplateHtml,
                request.TemplateCss);

            _schemas.Update(existing);
            schema = existing;
        }

        await _uow.SaveChangesAsync(ct);

        return new SchemaTemplateDto(
            schema.Id,
            schema.FolderId,
            schema.SchemaYaml,
            schema.TemplateHtml,
            schema.TemplateCss,
            schema.UpdatedAt);
    }
}