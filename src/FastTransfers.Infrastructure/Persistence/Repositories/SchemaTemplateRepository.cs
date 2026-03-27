using FastTransfers.Domain.Entities;
using FastTransfers.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastTransfers.Infrastructure.Persistence.Repositories
{
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
}
