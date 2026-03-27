using FastTransfers.Domain.Entities;

namespace FastTransfers.Domain.Interfaces
{
    public interface IFolderRepository
    {
        Task<Folder?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<Folder?> GetByIdWithProjectAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<Folder>> GetByProjectAsync(Guid projectId, CancellationToken ct = default);
        Task AddAsync(Folder folder, CancellationToken ct = default);
        void Update(Folder folder);
        void Delete(Folder folder);
    }
}
