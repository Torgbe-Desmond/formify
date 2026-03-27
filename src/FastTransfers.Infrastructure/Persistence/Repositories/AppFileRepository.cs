using FastTransfers.Domain.Entities;
using FastTransfers.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastTransfers.Infrastructure.Persistence.Repositories
{
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
}
