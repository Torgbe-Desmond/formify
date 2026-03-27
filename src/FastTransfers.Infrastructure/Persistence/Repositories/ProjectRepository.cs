using FastTransfers.Domain.Entities;
using FastTransfers.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastTransfers.Infrastructure.Persistence.Repositories
{
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
}
