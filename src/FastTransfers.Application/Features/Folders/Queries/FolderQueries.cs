//using FastTransfers.Application.DTOs;
//using FastTransfers.Domain.Entities;
//using FastTransfers.Domain.Exceptions;
//using FastTransfers.Domain.Interfaces;
//using MediatR;

//namespace FastTransfers.Application.Features.Folders.Queries;

//// ── Get folders by project ────────────────────────────────────────
//public record GetFoldersQuery(Guid ProjectId, Guid UserId) : IRequest<IReadOnlyList<FolderDto>>;

//public class GetFoldersQueryHandler : IRequestHandler<GetFoldersQuery, IReadOnlyList<FolderDto>>
//{
//    private readonly IFolderRepository _folders;
//    private readonly IProjectRepository _projects;
//    private readonly ISchemaTemplateRepository _schemas;
//    private readonly IAppFileRepository _files;

//    public GetFoldersQueryHandler(IFolderRepository folders,
//                                  IProjectRepository projects,
//                                  ISchemaTemplateRepository schemas,
//                                  IAppFileRepository files)
//    {
//        _folders = folders;
//        _projects = projects;
//        _schemas  = schemas;
//        _files    = files;
//    }

//    public async Task<IReadOnlyList<FolderDto>> Handle(GetFoldersQuery request, CancellationToken ct)
//    {
//        var project = await _projects.GetByIdAsync(request.ProjectId, ct)
//            ?? throw new NotFoundException(nameof(Project), request.ProjectId);

//        project.EnsureOwnedBy(request.UserId);

//        var folders = await _folders.GetByProjectAsync(request.ProjectId, ct);

//        var dtos = new List<FolderDto>();

//        foreach (var f in folders)
//        {
//            var schema = await _schemas.GetByFolderIdAsync(f.Id, ct);
//            var files  = await _files.GetByFolderAsync(f.Id, ct);
//            dtos.Add(new FolderDto(f.Id, f.Name, f.ProjectId,
//                                   f.CreatedAt, f.UpdatedAt,
//                                   schema is not null, files.Count));
//        }

//        return dtos;
//    }
//}


//// --- Get Folder by id

//public record GetFolderNameByIdQuery(Guid ProjectId, Guid UserId) : IRequest<IReadOnlyList<FolderDto>>;

//public class GetFolderNameByIdQueryHandler : IRequestHandler<GetFoldersQuery, IReadOnlyList<FolderDto>>
//{
//    private readonly IFolderRepository _folders;
//    private readonly IProjectRepository _projects;
//    private readonly ISchemaTemplateRepository _schemas;
//    private readonly IAppFileRepository _files;

//    public GetFolderNameByIdQueryHandler(IFolderRepository folders,
//                                  IProjectRepository projects,
//                                  ISchemaTemplateRepository schemas,
//                                  IAppFileRepository files)
//    {
//        _folders = folders;
//        _projects = projects;
//        _schemas = schemas;
//        _files = files;
//    }

//    public async Task<IReadOnlyList<FolderDto>> Handle(GetFoldersQuery request, CancellationToken ct)
//    {
//        var project = await _projects.GetByIdAsync(request.ProjectId, ct)
//            ?? throw new NotFoundException(nameof(Project), request.ProjectId);

//        project.EnsureOwnedBy(request.UserId);

//        var folders = await _folders.GetByProjectAsync(request.ProjectId, ct);

//        var dtos = new List<FolderDto>();

//        foreach (var f in folders)
//        {
//            var schema = await _schemas.GetByFolderIdAsync(f.Id, ct);
//            var files = await _files.GetByFolderAsync(f.Id, ct);
//            dtos.Add(new FolderDto(f.Id, f.Name, f.ProjectId,
//                                   f.CreatedAt, f.UpdatedAt,
//                                   schema is not null, files.Count));
//        }

//        return dtos;
//    }
//}