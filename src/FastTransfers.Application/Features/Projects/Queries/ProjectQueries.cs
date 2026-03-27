using FastTransfers.Application.DTOs;
using FastTransfers.Domain.Entities;
using FastTransfers.Domain.Exceptions;
using FastTransfers.Domain.Interfaces;
using MediatR;

namespace FastTransfers.Application.Features.Projects.Queries;

// ── Get all projects for a user ───────────────────────────────────
public record GetProjectsQuery(Guid UserId) : IRequest<IReadOnlyList<ProjectDto>>;

public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, IReadOnlyList<ProjectDto>>
{
    private readonly IProjectRepository _projects;
    private readonly IFolderRepository _folders;

    public GetProjectsQueryHandler(IProjectRepository projects, IFolderRepository folders)
    {
        _projects = projects;
        _folders  = folders;
    }

    public async Task<IReadOnlyList<ProjectDto>> Handle(GetProjectsQuery request, CancellationToken ct)
    {
        var projects = await _projects.GetByOwnerAsync(request.UserId, ct);

        var dtos = new List<ProjectDto>();

        foreach (var p in projects)
        {
            var folders = await _folders.GetByProjectAsync(p.Id, ct);
            dtos.Add(new ProjectDto(p.Id, p.Name, p.CreatedAt, p.UpdatedAt, folders.Count));
        }

        return dtos;
    }
}

// ── Get single project ────────────────────────────────────────────
public record GetProjectByIdQuery(Guid ProjectId, Guid UserId) : IRequest<ProjectDto>;

public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectDto>
{
    private readonly IProjectRepository _projects;
    private readonly IFolderRepository _folders;

    public GetProjectByIdQueryHandler(IProjectRepository projects, IFolderRepository folders)
    {
        _projects = projects;
        _folders  = folders;
    }

    public async Task<ProjectDto> Handle(GetProjectByIdQuery request, CancellationToken ct)
    {
        var project = await _projects.GetByIdAsync(request.ProjectId, ct)
            ?? throw new NotFoundException(nameof(Project), request.ProjectId);

        project.EnsureOwnedBy(request.UserId);

        var folders = await _folders.GetByProjectAsync(project.Id, ct);

        return new ProjectDto(project.Id, project.Name, project.CreatedAt, project.UpdatedAt, folders.Count);
    }
}
