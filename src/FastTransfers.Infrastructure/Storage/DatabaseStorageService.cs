using FastTransfers.Domain.Entities;
using FastTransfers.Domain.Exceptions;
using FastTransfers.Domain.Interfaces;
using FastTransfers.Infrastructure.Persistence;

namespace FastTransfers.Infrastructure.Storage;

/// <summary>
/// Stores file content in the SQL Server FileContents table.
/// StorageKey = FileContent.Id (as string).
/// Used when no external blob provider is configured.
/// </summary>
public class DatabaseStorageService : IFileStorageService
{
    private readonly AppDbContext _db;
    private readonly IFileContentRepository _repo;
    private readonly IUnitOfWork _uow;

    public DatabaseStorageService(AppDbContext db,
                                   IFileContentRepository repo,
                                   IUnitOfWork uow)
    {
        _db   = db;
        _repo = repo;
        _uow  = uow;
    }

    public async Task<string> UploadAsync(string content,
                                          string fileName,
                                          string contentType = "text/html",
                                          CancellationToken ct = default)
    {
        var record = FileContent.Create(content);
        await _repo.AddAsync(record, ct);
        await _uow.SaveChangesAsync(ct);

        // StorageKey is just the GUID of the FileContent row
        return record.Id.ToString();
    }

    public async Task<string> DownloadAsync(string storageKey, CancellationToken ct = default)
    {
        if (!Guid.TryParse(storageKey, out var id))
            throw new DomainException($"Invalid storage key: {storageKey}");

        var record = await _repo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"File content '{storageKey}' not found.");

        return record.Content;
    }

    public async Task DeleteAsync(string storageKey, CancellationToken ct = default)
    {
        if (!Guid.TryParse(storageKey, out var id)) return;

        var record = await _repo.GetByIdAsync(id, ct);
        if (record is null) return;

        _repo.Delete(record);
        await _uow.SaveChangesAsync(ct);
    }
}
