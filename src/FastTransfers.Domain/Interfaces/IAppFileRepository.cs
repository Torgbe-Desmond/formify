using FastTransfers.Domain.Entities;

namespace FastTransfers.Domain.Interfaces
{
    public interface IAppFileRepository
    {
        Task<AppFile?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<AppFile?> GetByIdWithMetadataAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<AppFile>> GetByFolderAsync(Guid folderId, CancellationToken ct = default);
        Task AddAsync(AppFile file, CancellationToken ct = default);
        void Update(AppFile file);
        void Delete(AppFile file);
    }
}
