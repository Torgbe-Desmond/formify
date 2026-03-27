namespace FastTransfers.Application.DTOs;

// ── Auth ─────────────────────────────────────────────────────────
public record AuthResponse(string Token, UserDto User);

// ── User ─────────────────────────────────────────────────────────
public record UserDto(Guid Id, string Name, string Email);

// ── Project ──────────────────────────────────────────────────────
public record ProjectDto(
    Guid Id,
    string Name,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    int FolderCount);

// ── Folder ───────────────────────────────────────────────────────
public record FolderDto(
    Guid Id,
    string Name,
    Guid ProjectId,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    bool HasSchema,
    int FileCount);

// ── Schema ───────────────────────────────────────────────────────
public record SchemaTemplateDto(
    Guid Id,
    Guid FolderId,
    string SchemaYaml,
    string TemplateHtml,
    string TemplateCss,
    DateTime UpdatedAt);

// ── File ─────────────────────────────────────────────────────────
public record AppFileDto(
    Guid Id,
    string Name,
    Guid FolderId,
    string ContentType,
    long SizeBytes,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    Dictionary<string, string> Metadata);

public record AppFileListDto(
    Guid Id,
    string Name,
    Guid FolderId,
    long SizeBytes,
    DateTime CreatedAt,
    DateTime UpdatedAt);
