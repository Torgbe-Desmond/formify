using FastTransfers.Domain.Entities;
using FastTransfers.Domain.Interfaces;
using FastTransfers.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FastTransfers.Infrastructure.Persistence.Repositories;

// ── User ─────────────────────────────────────────────────────────
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) => _db = db;

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => _db.Users.FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), ct);

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
        => _db.Users.AnyAsync(u => u.Email == email.ToLowerInvariant(), ct);

    public async Task AddAsync(User user, CancellationToken ct = default)
        => await _db.Users.AddAsync(user, ct);

    public void Update(User user)
        => _db.Users.Update(user);
}

// ── Project ──────────────────────────────────────────────────────
public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _db;
    public ProjectRepository(AppDbContext db) => _db = db;

    public Task<Project?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Projects.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IReadOnlyList<Project>> GetByOwnerAsync(Guid ownerId, CancellationToken ct = default)
        => await _db.Projects
            .Where(p => p.OwnerId == ownerId)
            .OrderByDescending(p => p.UpdatedAt)
            .ToListAsync(ct);

    public async Task AddAsync(Project project, CancellationToken ct = default)
        => await _db.Projects.AddAsync(project, ct);

    public void Update(Project project)
        => _db.Projects.Update(project);

    public void Delete(Project project)
        => _db.Projects.Remove(project);
}

// ── Folder ───────────────────────────────────────────────────────
public class FolderRepository : IFolderRepository
{
    private readonly AppDbContext _db;
    public FolderRepository(AppDbContext db) => _db = db;

    public Task<Folder?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Folders.FirstOrDefaultAsync(f => f.Id == id, ct);

    public Task<Folder?> GetByIdWithProjectAsync(Guid id, CancellationToken ct = default)
        => _db.Folders
            .Include(f => f.Project)
            .FirstOrDefaultAsync(f => f.Id == id, ct);

    public async Task<IReadOnlyList<Folder>> GetByProjectAsync(Guid projectId, CancellationToken ct = default)
        => await _db.Folders
            .Where(f => f.ProjectId == projectId)
            .OrderBy(f => f.Name)
            .ToListAsync(ct);

    public async Task AddAsync(Folder folder, CancellationToken ct = default)
        => await _db.Folders.AddAsync(folder, ct);

    public void Update(Folder folder)
        => _db.Folders.Update(folder);

    public void Delete(Folder folder)
        => _db.Folders.Remove(folder);
}

// ── SchemaTemplate ───────────────────────────────────────────────
public class SchemaTemplateRepository : ISchemaTemplateRepository
{
    private readonly AppDbContext _db;
    public SchemaTemplateRepository(AppDbContext db) => _db = db;

    public Task<SchemaTemplate?> GetByFolderIdAsync(Guid folderId, CancellationToken ct = default)
        => _db.SchemaTemplates.FirstOrDefaultAsync(s => s.FolderId == folderId, ct);

    public async Task AddAsync(SchemaTemplate schema, CancellationToken ct = default)
        => await _db.SchemaTemplates.AddAsync(schema, ct);

    public void Update(SchemaTemplate schema)
        => _db.SchemaTemplates.Update(schema);
}

// ── AppFile ──────────────────────────────────────────────────────
public class AppFileRepository : IAppFileRepository
{
    private readonly AppDbContext _db;
    public AppFileRepository(AppDbContext db) => _db = db;

    public Task<AppFile?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.AppFiles.FirstOrDefaultAsync(f => f.Id == id, ct);

    public Task<AppFile?> GetByIdWithMetadataAsync(Guid id, CancellationToken ct = default)
        => _db.AppFiles
            .Include(f => f.Metadata)
            .FirstOrDefaultAsync(f => f.Id == id, ct);

    public async Task<IReadOnlyList<AppFile>> GetByFolderAsync(Guid folderId, CancellationToken ct = default)
        => await _db.AppFiles
            .Where(f => f.FolderId == folderId)
            .OrderByDescending(f => f.UpdatedAt)
            .ToListAsync(ct);

    public async Task AddAsync(AppFile file, CancellationToken ct = default)
        => await _db.AppFiles.AddAsync(file, ct);

    public void Update(AppFile file)
        => _db.AppFiles.Update(file);

    public void Delete(AppFile file)
        => _db.AppFiles.Remove(file);
}

// ── FileContent ──────────────────────────────────────────────────
public class FileContentRepository : IFileContentRepository
{
    private readonly AppDbContext _db;
    public FileContentRepository(AppDbContext db) => _db = db;

    public Task<FileContent?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.FileContents.FirstOrDefaultAsync(f => f.Id == id, ct);

    public async Task AddAsync(FileContent content, CancellationToken ct = default)
        => await _db.FileContents.AddAsync(content, ct);

    public void Update(FileContent content)
        => _db.FileContents.Update(content);

    public void Delete(FileContent content)
        => _db.FileContents.Remove(content);
}
