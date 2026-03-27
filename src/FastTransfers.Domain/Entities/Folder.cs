using FastTransfers.Domain.Common;
using FastTransfers.Domain.Exceptions;

namespace FastTransfers.Domain.Entities;

public class Folder : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public Guid ProjectId { get; private set; }

    // Navigation
    public Project Project { get; private set; } = null!;
    public SchemaTemplate? Schema { get; private set; }
    public ICollection<AppFile> Files { get; private set; } = new List<AppFile>();

    private Folder() { } // EF

    public static Folder Create(string name, Guid projectId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new Folder
        {
            Name      = name.Trim(),
            ProjectId = projectId
        };
    }

    public void Rename(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
        SetUpdated();
    }

    /// <summary>
    /// Ownership is always resolved through the parent project.
    /// </summary>
    public void EnsureProjectOwnedBy(Guid userId)
    {
        if (Project is null)
            throw new DomainException("Project must be loaded before checking ownership.");

        Project.EnsureOwnedBy(userId);
    }
}
