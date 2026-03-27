using System.Text;
using FastTransfers.Application.DTOs;
using FastTransfers.Domain.Entities;
using FastTransfers.Domain.Enums;
using FastTransfers.Domain.Exceptions;
using FastTransfers.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace FastTransfers.Application.Features.Files.Commands;

// ── Create ───────────────────────────────────────────────────────
public record CreateFileCommand(
    string Name,
    Guid FolderId,
    Guid UserId,
    string RenderedHtml,
    Dictionary<string, string> Metadata) : IRequest<AppFileDto>;

public class CreateFileCommandValidator : AbstractValidator<CreateFileCommand>
{
    public CreateFileCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(300);
        RuleFor(x => x.FolderId).NotEmpty();
        RuleFor(x => x.RenderedHtml).NotEmpty();
    }
}

public class CreateFileCommandHandler : IRequestHandler<CreateFileCommand, AppFileDto>
{
    private readonly IAppFileRepository _files;
    private readonly IFolderRepository _folders;
    private readonly IFileStorageService _storage;
    private readonly IUnitOfWork _uow;

    public CreateFileCommandHandler(IAppFileRepository files,
                                    IFolderRepository folders,
                                    IFileStorageService storage,
                                    IUnitOfWork uow)
    {
        _files   = files;
        _folders = folders;
        _storage = storage;
        _uow     = uow;
    }

    public async Task<AppFileDto> Handle(CreateFileCommand request, CancellationToken ct)
    {
        var folder = await _folders.GetByIdWithProjectAsync(request.FolderId, ct)
            ?? throw new NotFoundException(nameof(Folder), request.FolderId);

        folder.EnsureProjectOwnedBy(request.UserId);

        // Upload content to configured storage provider
        var storageKey = await _storage.UploadAsync(request.RenderedHtml,
                                                     $"{Guid.NewGuid()}.html",
                                                     "text/html", ct);

        var sizeBytes = Encoding.UTF8.GetByteCount(request.RenderedHtml);

        var file = AppFile.Create(request.Name, request.FolderId,
                                  storageKey, StorageProvider.Database,
                                  sizeBytes);

        await _files.AddAsync(file, ct);

        // Save metadata fields
        foreach (var (key, value) in request.Metadata)
        {
            var meta = AppFileMetadata.Create(file.Id, key, value);
            file.Metadata.Add(meta);
        }

        await _uow.SaveChangesAsync(ct);

        return ToDto(file, request.Metadata);
    }

    private static AppFileDto ToDto(AppFile file, Dictionary<string, string> metadata)
        => new(file.Id, file.Name, file.FolderId, file.ContentType,
               file.SizeBytes, file.CreatedAt, file.UpdatedAt, metadata);
}

// ── Update ───────────────────────────────────────────────────────
public record UpdateFileCommand(
    Guid FileId,
    Guid UserId,
    string Name,
    string RenderedHtml,
    Dictionary<string, string> Metadata) : IRequest<AppFileDto>;

public class UpdateFileCommandValidator : AbstractValidator<UpdateFileCommand>
{
    public UpdateFileCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(300);
        RuleFor(x => x.RenderedHtml).NotEmpty();
    }
}

public class UpdateFileCommandHandler : IRequestHandler<UpdateFileCommand, AppFileDto>
{
    private readonly IAppFileRepository _files;
    private readonly IFolderRepository _folders;
    private readonly IFileStorageService _storage;
    private readonly IUnitOfWork _uow;

    public UpdateFileCommandHandler(IAppFileRepository files,
                                    IFolderRepository folders,
                                    IFileStorageService storage,
                                    IUnitOfWork uow)
    {
        _files   = files;
        _folders = folders;
        _storage = storage;
        _uow     = uow;
    }

    public async Task<AppFileDto> Handle(UpdateFileCommand request, CancellationToken ct)
    {
        var file = await _files.GetByIdWithMetadataAsync(request.FileId, ct)
            ?? throw new NotFoundException(nameof(AppFile), request.FileId);

        var folder = await _folders.GetByIdWithProjectAsync(file.FolderId, ct)
            ?? throw new NotFoundException(nameof(Folder), file.FolderId);

        folder.EnsureProjectOwnedBy(request.UserId);

        // Delete old content and upload new
        await _storage.DeleteAsync(file.StorageKey, ct);
        var newKey    = await _storage.UploadAsync(request.RenderedHtml,
                                                   $"{Guid.NewGuid()}.html",
                                                   "text/html", ct);
        var sizeBytes = Encoding.UTF8.GetByteCount(request.RenderedHtml);

        file.Rename(request.Name);
        file.UpdateStorageReference(newKey, sizeBytes);

        // Replace metadata
        file.Metadata.Clear();
        foreach (var (key, value) in request.Metadata)
            file.Metadata.Add(AppFileMetadata.Create(file.Id, key, value));

        _files.Update(file);
        await _uow.SaveChangesAsync(ct);

        return new AppFileDto(file.Id, file.Name, file.FolderId, file.ContentType,
                              file.SizeBytes, file.CreatedAt, file.UpdatedAt, request.Metadata);
    }
}

// ── Delete ───────────────────────────────────────────────────────
public record DeleteFileCommand(Guid FileId, Guid UserId) : IRequest;

public class DeleteFileCommandHandler : IRequestHandler<DeleteFileCommand>
{
    private readonly IAppFileRepository _files;
    private readonly IFolderRepository _folders;
    private readonly IFileStorageService _storage;
    private readonly IUnitOfWork _uow;

    public DeleteFileCommandHandler(IAppFileRepository files,
                                    IFolderRepository folders,
                                    IFileStorageService storage,
                                    IUnitOfWork uow)
    {
        _files   = files;
        _folders = folders;
        _storage = storage;
        _uow     = uow;
    }

    public async Task Handle(DeleteFileCommand request, CancellationToken ct)
    {
        var file = await _files.GetByIdAsync(request.FileId, ct)
            ?? throw new NotFoundException(nameof(AppFile), request.FileId);

        var folder = await _folders.GetByIdWithProjectAsync(file.FolderId, ct)
            ?? throw new NotFoundException(nameof(Folder), file.FolderId);

        folder.EnsureProjectOwnedBy(request.UserId);

        await _storage.DeleteAsync(file.StorageKey, ct);

        _files.Delete(file);
        await _uow.SaveChangesAsync(ct);
    }
}
