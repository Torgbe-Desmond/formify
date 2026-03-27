using FastTransfers.Domain.Common;

namespace FastTransfers.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;

    // Navigation
    public ICollection<Project> Projects { get; private set; } = new List<Project>();

    private User() { } // EF

    public static User Create(string name, string email, string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

        return new User
        {
            Name          = name.Trim(),
            Email         = email.Trim().ToLowerInvariant(),
            PasswordHash  = passwordHash
        };
    }

    public void UpdateName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
        SetUpdated();
    }

    public void UpdatePasswordHash(string hash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(hash);
        PasswordHash = hash;
        SetUpdated();
    }
}
