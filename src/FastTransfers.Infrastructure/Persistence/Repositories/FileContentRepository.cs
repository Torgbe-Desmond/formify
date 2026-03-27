using FastTransfers.Domain.Entities;
using FastTransfers.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastTransfers.Infrastructure.Persistence.Repositories
{
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

}
