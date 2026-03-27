using FastTransfers.Domain.Entities;

namespace FastTransfers.Domain.Interfaces
{
    public interface IFileContentRepository
    {
        Task<FileContent?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(FileContent content, CancellationToken ct = default);
        void Update(FileContent content);
        void Delete(FileContent content);
    }
}
