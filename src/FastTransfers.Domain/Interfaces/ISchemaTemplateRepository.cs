using FastTransfers.Domain.Entities;

namespace FastTransfers.Domain.Interfaces
{
    public interface ISchemaTemplateRepository
    {
        Task<SchemaTemplate?> GetByFolderIdAsync(Guid folderId, CancellationToken ct = default);
        Task AddAsync(SchemaTemplate schema, CancellationToken ct = default);
        void Update(SchemaTemplate schema);
    }
}
