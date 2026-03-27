using FastTransfers.Domain.Exceptions;
using FastTransfers.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FastTransfers.Infrastructure.Storage;

/// <summary>
/// Stores file content on the local filesystem.
/// StorageKey = relative path e.g. "files/2026/03/abc-123.html"
/// Root folder is configured via Storage:LocalRootPath in appsettings.
/// </summary>
public class LocalStorageService : IFileStorageService
{
    private readonly string _rootPath;

    public LocalStorageService(IConfiguration config)
    {
        _rootPath = config["Storage:LocalRootPath"]
            ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");

        Directory.CreateDirectory(_rootPath);
    }

    public async Task<string> UploadAsync(string content,
                                          string fileName,
                                          string contentType = "text/html",
                                          CancellationToken ct = default)
    {
        var now        = DateTime.UtcNow;
        var relativePath = Path.Combine("files", now.Year.ToString(), now.Month.ToString("D2"), fileName)
                              .Replace("\\", "/");

        var fullPath = Path.Combine(_rootPath, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await File.WriteAllTextAsync(fullPath, content, ct);

        return relativePath;
    }

    public async Task<string> DownloadAsync(string storageKey, CancellationToken ct = default)
    {
        var fullPath = Path.Combine(_rootPath, storageKey);

        if (!File.Exists(fullPath))
            throw new NotFoundException($"File not found at path: {storageKey}");

        return await File.ReadAllTextAsync(fullPath, ct);
    }

    public Task DeleteAsync(string storageKey, CancellationToken ct = default)
    {
        var fullPath = Path.Combine(_rootPath, storageKey);

        if (File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }
}
