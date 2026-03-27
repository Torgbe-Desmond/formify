namespace FastTransfers.Domain.Interfaces;

/// <summary>
/// Storage-agnostic interface for reading and writing file content.
/// Implemented in Infrastructure for each provider (AzureBlob, S3, Local, Database).
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Upload content and return the StorageKey to save on AppFile.
    /// e.g. "files/2026/03/abc-123.html"
    /// </summary>
    Task<string> UploadAsync(string content,
                             string fileName,
                             string contentType = "text/html",
                             CancellationToken ct = default);

    /// <summary>Download and return raw content using the StorageKey.</summary>
    Task<string> DownloadAsync(string storageKey, CancellationToken ct = default);

    /// <summary>Delete the file from storage using the StorageKey.</summary>
    Task DeleteAsync(string storageKey, CancellationToken ct = default);
}
