using FastTransfers.Domain.Entities;
using FastTransfers.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastTransfers.Infrastructure.Persistence.Repositories
{
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
}
