using FastTransfers.Domain.Entities;

namespace FastTransfers.Domain.Interfaces;

//public interface IUserRepository
//{
//    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
//    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
//    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);
//    Task AddAsync(User user, CancellationToken ct = default);
//    void Update(User user);
//}

//public interface IProjectRepository
//{
//    Task<Project?> GetByIdAsync(Guid id, CancellationToken ct = default);
//    Task<IReadOnlyList<Project>> GetByOwnerAsync(Guid ownerId, CancellationToken ct = default);
//    Task AddAsync(Project project, CancellationToken ct = default);
//    void Update(Project project);
//    void Delete(Project project);
//}

//public interface IFolderRepository
//{
//    Task<Folder?> GetByIdAsync(Guid id, CancellationToken ct = default);
//    Task<Folder?> GetByIdWithProjectAsync(Guid id, CancellationToken ct = default);
//    Task<IReadOnlyList<Folder>> GetByProjectAsync(Guid projectId, CancellationToken ct = default);
//    Task AddAsync(Folder folder, CancellationToken ct = default);
//    void Update(Folder folder);
//    void Delete(Folder folder);
//}

//public interface ISchemaTemplateRepository
//{
//    Task<SchemaTemplate?> GetByFolderIdAsync(Guid folderId, CancellationToken ct = default);
//    Task AddAsync(SchemaTemplate schema, CancellationToken ct = default);
//    void Update(SchemaTemplate schema);
//}

//public interface IAppFileRepository
//{
//    Task<AppFile?> GetByIdAsync(Guid id, CancellationToken ct = default);
//    Task<AppFile?> GetByIdWithMetadataAsync(Guid id, CancellationToken ct = default);
//    Task<IReadOnlyList<AppFile>> GetByFolderAsync(Guid folderId, CancellationToken ct = default);
//    Task AddAsync(AppFile file, CancellationToken ct = default);
//    void Update(AppFile file);
//    void Delete(AppFile file);
//}

//public interface IFileContentRepository
//{
//    Task<FileContent?> GetByIdAsync(Guid id, CancellationToken ct = default);
//    Task AddAsync(FileContent content, CancellationToken ct = default);
//    void Update(FileContent content);
//    void Delete(FileContent content);
//}

//public interface IUnitOfWork
//{
//    Task<int> SaveChangesAsync(CancellationToken ct = default);
//}
