using FastTransfers.Domain.Common;

namespace FastTransfers.Domain.Entities;

/// <summary>
/// Stores the structured form data used to generate an AppFile.
/// Each field from the schema becomes one row here.
/// Value is JSON-encoded to support strings, numbers, booleans, and arrays.
/// e.g. Key = "completedTasks", Value = "[{\"category\":\"Dev\",\"remark\":\"...\"}]"
/// </summary>
public class AppFileMetadata : BaseEntity
{
    public Guid FileId { get; private set; }

    /// <summary>Matches a field key from the SchemaTemplate YAML.</summary>
    public string Key { get; private set; } = string.Empty;

    /// <summary>JSON-encoded value.</summary>
    public string Value { get; private set; } = string.Empty;

    // Navigation
    public AppFile File { get; private set; } = null!;

    private AppFileMetadata() { } // EF

    public static AppFileMetadata Create(Guid fileId, string key, string jsonValue)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        return new AppFileMetadata
        {
            FileId = fileId,
            Key    = key.Trim(),
            Value  = jsonValue ?? string.Empty
        };
    }

    public void UpdateValue(string jsonValue)
    {
        Value = jsonValue ?? string.Empty;
        SetUpdated();
    }
}
