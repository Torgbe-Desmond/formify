using FastTransfers.Domain.Common;
using FastTransfers.Domain.Enums;

namespace FastTransfers.Domain.Entities;

/// <summary>
/// A rendered document created from a folder's SchemaTemplate.
/// The actual HTML content is stored externally (blob, S3, local, or DB)
/// and referenced via StorageKey + StorageProvider.
/// Metadata (the form data used to generate it) is always in SQL Server.
/// </summary>
public class AppFile : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public Guid FolderId { get; private set; }

    // ── Storage reference ───────────────────────────────────
    /// <summary>
    /// Path within the storage provider.
    /// e.g. "files/2026/03/abc-123.html" for blob/S3/local,
    /// or the FileContent.Id for Database provider.
    /// </summary>
    public string StorageKey { get; private set; } = string.Empty;

    public StorageProvider StorageProvider { get; private set; }

    public string ContentType { get; private set; } = "text/html";

    /// <summary>File size in bytes. Stored so listings never need to fetch content.</summary>
    public long SizeBytes { get; private set; }

    // Navigation
    public Folder Folder { get; private set; } = null!;
    public ICollection<AppFileMetadata> Metadata { get; private set; } = new List<AppFileMetadata>();

    private AppFile() { } // EF

    public static AppFile Create(string name,
                                 Guid folderId,
                                 string storageKey,
                                 StorageProvider storageProvider,
                                 long sizeBytes,
                                 string contentType = "text/html")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(storageKey);

        return new AppFile
        {
            Name            = name.Trim(),
            FolderId        = folderId,
            StorageKey      = storageKey,
            StorageProvider = storageProvider,
            SizeBytes       = sizeBytes,
            ContentType     = contentType
        };
    }

    public void Rename(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
        SetUpdated();
    }

    public void UpdateStorageReference(string storageKey, long sizeBytes)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(storageKey);
        StorageKey = storageKey;
        SizeBytes  = sizeBytes;
        SetUpdated();
    }
}
