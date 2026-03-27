using FastTransfers.Domain.Exceptions;
using FastTransfers.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FastTransfers.Infrastructure.Storage;

/// <summary>
/// Stores file content as documents in a MongoDB collection.
/// StorageKey = MongoDB ObjectId string (e.g. "507f1f77bcf86cd799439011").
///
/// appsettings.json config block:
/// "Storage": {
///   "Provider": "MongoDB",
///   "MongoDB": {
///     "ConnectionString": "mongodb://localhost:27017",
///     "DatabaseName": "FastTransfersFiles",
///     "CollectionName": "fileContents"
///   }
/// }
/// </summary>
public class MongoDbStorageService : IFileStorageService
{
    private readonly IMongoCollection<FileContentDocument> _collection;

    public MongoDbStorageService(IConfiguration config)
    {
        var connectionString = config["Storage:MongoDB:ConnectionString"]
            ?? throw new InvalidOperationException("Storage:MongoDB:ConnectionString is not configured.");

        var databaseName = config["Storage:MongoDB:DatabaseName"]
            ?? "FastTransfersFiles";

        var collectionName = config["Storage:MongoDB:CollectionName"]
            ?? "fileContents";

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);

        _collection = database.GetCollection<FileContentDocument>(collectionName);

        // Ensure index on CreatedAt for efficient date-range queries / TTL policies
        var indexModel = new CreateIndexModel<FileContentDocument>(
            Builders<FileContentDocument>.IndexKeys.Ascending(d => d.CreatedAt));

        _collection.Indexes.CreateOne(indexModel);
    }

    /// <summary>
    /// Inserts a new document into MongoDB and returns its ObjectId as the StorageKey.
    /// </summary>
    public async Task<string> UploadAsync(string content,
                                          string fileName,
                                          string contentType = "text/html",
                                          CancellationToken ct = default)
    {
        var document = new FileContentDocument
        {
            Id = ObjectId.GenerateNewId(),
            FileName = fileName,
            ContentType = contentType,
            Content = content,
            SizeBytes = System.Text.Encoding.UTF8.GetByteCount(content),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await _collection.InsertOneAsync(document, cancellationToken: ct);

        // StorageKey = the ObjectId string so AppFile can reference it
        return document.Id.ToString();
    }

    /// <summary>
    /// Fetches a document from MongoDB using its ObjectId StorageKey.
    /// </summary>
    public async Task<string> DownloadAsync(string storageKey, CancellationToken ct = default)
    {
        var id = ParseObjectId(storageKey);

        var document = await _collection
            .Find(d => d.Id == id)
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException($"File content '{storageKey}' not found in MongoDB.");

        return document.Content;
    }

    /// <summary>
    /// Deletes a document from MongoDB using its ObjectId StorageKey.
    /// </summary>
    public async Task DeleteAsync(string storageKey, CancellationToken ct = default)
    {
        if (!ObjectId.TryParse(storageKey, out var id)) return;

        await _collection.DeleteOneAsync(d => d.Id == id, ct);
    }

    // ── Private helpers ───────────────────────────────────────────

    private static ObjectId ParseObjectId(string storageKey)
    {
        if (!ObjectId.TryParse(storageKey, out var id))
            throw new DomainException($"Invalid MongoDB storage key: '{storageKey}'.");

        return id;
    }
}

/// <summary>
/// MongoDB document schema for stored file content.
/// </summary>
public class FileContentDocument
{
    public ObjectId Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = "text/html";
    public string Content { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}