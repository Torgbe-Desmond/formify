using FastTransfers.Domain.Entities;

namespace FastTransfers.Domain.Interfaces
{
    public interface IProjectRepository
    {
        Task<Project?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<Project>> GetByOwnerAsync(Guid ownerId, CancellationToken ct = default);
        Task AddAsync(Project project, CancellationToken ct = default);
        void Update(Project project);
        void Delete(Project project);
    }
}
