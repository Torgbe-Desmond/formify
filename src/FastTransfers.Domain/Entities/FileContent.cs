using FastTransfers.Domain.Common;

namespace FastTransfers.Domain.Entities;

/// <summary>
/// Holds the actual file bytes when StorageProvider = Database.
/// Kept in a separate table so AppFile listings never load content unnecessarily.
/// StorageKey on AppFile matches this entity's Id.
/// </summary>
public class FileContent : BaseEntity
{
    /// <summary>Raw HTML content of the rendered document.</summary>
    public string Content { get; private set; } = string.Empty;

    private FileContent() { } // EF

    public static FileContent Create(string content)
    {
        return new FileContent { Content = content };
    }

    public void Update(string content)
    {
        Content = content;
        SetUpdated();
    }
}
