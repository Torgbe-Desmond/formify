using FastTransfers.Application.DTOs;
using FastTransfers.Domain.Entities;
using FastTransfers.Domain.Exceptions;
using FastTransfers.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace FastTransfers.Application.Features.Folders.Commands;

// ── Create ───────────────────────────────────────────────────────
public record CreateFolderCommand(string Name, Guid ProjectId, Guid UserId) : IRequest<FolderDto>;

public class CreateFolderCommandValidator : AbstractValidator<CreateFolderCommand>
{
    public CreateFolderCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}

public class CreateFolderCommandHandler : IRequestHandler<CreateFolderCommand, FolderDto>
{
    private readonly IFolderRepository _folders;
    private readonly IProjectRepository _projects;
    private readonly IUnitOfWork _uow;

    public CreateFolderCommandHandler(IFolderRepository folders,
                                      IProjectRepository projects,
                                      IUnitOfWork uow)
    {
        _folders  = folders;
        _projects = projects;
        _uow      = uow;
    }

    public async Task<FolderDto> Handle(CreateFolderCommand request, CancellationToken ct)
    {
        var project = await _projects.GetByIdAsync(request.ProjectId, ct)
            ?? throw new NotFoundException(nameof(Project), request.ProjectId);

        project.EnsureOwnedBy(request.UserId);

        var folder = Folder.Create(request.Name, request.ProjectId);

        await _folders.AddAsync(folder, ct);
        await _uow.SaveChangesAsync(ct);

        return new FolderDto(folder.Id, folder.Name, folder.ProjectId,
                             folder.CreatedAt, folder.UpdatedAt, false, 0);
    }
}

// ── Rename ───────────────────────────────────────────────────────
public record RenameFolderCommand(Guid FolderId, string Name, Guid UserId) : IRequest<FolderDto>;

public class RenameFolderCommandValidator : AbstractValidator<RenameFolderCommand>
{
    public RenameFolderCommandValidator()
        => RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
}

public class RenameFolderCommandHandler : IRequestHandler<RenameFolderCommand, FolderDto>
{
    private readonly IFolderRepository _folders;
    private readonly IAppFileRepository _files;
    private readonly ISchemaTemplateRepository _schemas;
    private readonly IUnitOfWork _uow;

    public RenameFolderCommandHandler(IFolderRepository folders,
                                      IAppFileRepository files,
                                      ISchemaTemplateRepository schemas,
                                      IUnitOfWork uow)
    {
        _folders = folders;
        _files   = files;
        _schemas = schemas;
        _uow     = uow;
    }

    public async Task<FolderDto> Handle(RenameFolderCommand request, CancellationToken ct)
    {
        var folder = await _folders.GetByIdWithProjectAsync(request.FolderId, ct)
            ?? throw new NotFoundException(nameof(Folder), request.FolderId);

        folder.EnsureProjectOwnedBy(request.UserId);
        folder.Rename(request.Name);

        _folders.Update(folder);
        await _uow.SaveChangesAsync(ct);

        var schema = await _schemas.GetByFolderIdAsync(folder.Id, ct);
        var files  = await _files.GetByFolderAsync(folder.Id, ct);

        return new FolderDto(folder.Id, folder.Name, folder.ProjectId,
                             folder.CreatedAt, folder.UpdatedAt,
                             schema is not null, files.Count);
    }
}

// ── Delete ───────────────────────────────────────────────────────
public record DeleteFolderCommand(Guid FolderId, Guid UserId) : IRequest;

public class DeleteFolderCommandHandler : IRequestHandler<DeleteFolderCommand>
{
    private readonly IFolderRepository _folders;
    private readonly IUnitOfWork _uow;

    public DeleteFolderCommandHandler(IFolderRepository folders, IUnitOfWork uow)
    {
        _folders = folders;
        _uow     = uow;
    }

    public async Task Handle(DeleteFolderCommand request, CancellationToken ct)
    {
        var folder = await _folders.GetByIdWithProjectAsync(request.FolderId, ct)
            ?? throw new NotFoundException(nameof(Folder), request.FolderId);

        folder.EnsureProjectOwnedBy(request.UserId);

        _folders.Delete(folder);
        await _uow.SaveChangesAsync(ct);
    }
}


