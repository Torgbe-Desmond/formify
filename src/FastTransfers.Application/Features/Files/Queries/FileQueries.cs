using FastTransfers.Application.DTOs;
using FastTransfers.Domain.Entities;
using FastTransfers.Domain.Exceptions;
using FastTransfers.Domain.Interfaces;
using MediatR;

namespace FastTransfers.Application.Features.Files.Queries;

// ── List files in folder ──────────────────────────────────────────
public record GetFilesQuery(Guid FolderId, Guid UserId) : IRequest<IReadOnlyList<AppFileListDto>>;

public class GetFilesQueryHandler : IRequestHandler<GetFilesQuery, IReadOnlyList<AppFileListDto>>
{
    private readonly IAppFileRepository _files;
    private readonly IFolderRepository _folders;

    public GetFilesQueryHandler(IAppFileRepository files, IFolderRepository folders)
    {
        _files   = files;
        _folders = folders;
    }

    public async Task<IReadOnlyList<AppFileListDto>> Handle(GetFilesQuery request, CancellationToken ct)
    {
        var folder = await _folders.GetByIdWithProjectAsync(request.FolderId, ct)
            ?? throw new NotFoundException(nameof(Folder), request.FolderId);

        folder.EnsureProjectOwnedBy(request.UserId);

        var files = await _files.GetByFolderAsync(request.FolderId, ct);

        return files.Select(f =>
            new AppFileListDto(f.Id, f.Name, f.FolderId, f.SizeBytes, f.CreatedAt, f.UpdatedAt))
            .ToList();
    }
}

// ── Get single file with content ──────────────────────────────────
public record GetFileQuery(Guid FileId, Guid UserId) : IRequest<(AppFileDto Dto, string Content)>;

public class GetFileQueryHandler : IRequestHandler<GetFileQuery, (AppFileDto Dto, string Content)>
{
    private readonly IAppFileRepository _files;
    private readonly IFolderRepository _folders;
    private readonly IFileStorageService _storage;

    public GetFileQueryHandler(IAppFileRepository files,
                               IFolderRepository folders,
                               IFileStorageService storage)
    {
        _files   = files;
        _folders = folders;
        _storage = storage;
    }

    public async Task<(AppFileDto Dto, string Content)> Handle(GetFileQuery request, CancellationToken ct)
    {
        var file = await _files.GetByIdWithMetadataAsync(request.FileId, ct)
            ?? throw new NotFoundException(nameof(AppFile), request.FileId);

        var folder = await _folders.GetByIdWithProjectAsync(file.FolderId, ct)
            ?? throw new NotFoundException(nameof(Folder), file.FolderId);

        folder.EnsureProjectOwnedBy(request.UserId);

        // Fetch actual content from storage
        var content = await _storage.DownloadAsync(file.StorageKey, ct);

        var metadata = file.Metadata.ToDictionary(m => m.Key, m => m.Value);

        var dto = new AppFileDto(file.Id, file.Name, file.FolderId, file.ContentType,
                                 file.SizeBytes, file.CreatedAt, file.UpdatedAt, metadata);

        return (dto, content);
    }
}
