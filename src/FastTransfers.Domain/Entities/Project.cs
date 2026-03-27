using FastTransfers.Domain.Common;
using FastTransfers.Domain.Exceptions;

namespace FastTransfers.Domain.Entities;

public class Project : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public Guid OwnerId { get; private set; }

    // Navigation
    public User Owner { get; private set; } = null!;
    public ICollection<Folder> Folders { get; private set; } = new List<Folder>();

    private Project() { } // EF

    public static Project Create(string name, Guid ownerId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new Project
        {
            Name    = name.Trim(),
            OwnerId = ownerId
        };
    }

    public void Rename(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
        SetUpdated();
    }

    public void EnsureOwnedBy(Guid userId)
    {
        if (OwnerId != userId)
            throw new UnauthorizedDomainException("You do not have access to this project.");
    }
}
