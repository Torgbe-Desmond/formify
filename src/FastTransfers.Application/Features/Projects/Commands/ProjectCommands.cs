using FastTransfers.Application.DTOs;
using FastTransfers.Domain.Entities;
using FastTransfers.Domain.Exceptions;
using FastTransfers.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace FastTransfers.Application.Features.Projects.Commands;

// ── Create ───────────────────────────────────────────────────────
public record CreateProjectCommand(string Name, Guid OwnerId) : IRequest<ProjectDto>;

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.OwnerId).NotEmpty();
    }
}

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectDto>
{
    private readonly IProjectRepository _projects;
    private readonly IUnitOfWork _uow;

    public CreateProjectCommandHandler(IProjectRepository projects, IUnitOfWork uow)
    {
        _projects = projects;
        _uow      = uow;
    }

    public async Task<ProjectDto> Handle(CreateProjectCommand request, CancellationToken ct)
    {
        var project = Project.Create(request.Name, request.OwnerId);

        await _projects.AddAsync(project, ct);
        await _uow.SaveChangesAsync(ct);

        return new ProjectDto(project.Id, project.Name, project.CreatedAt, project.UpdatedAt, 0);
    }
}

// ── Rename ───────────────────────────────────────────────────────
public record RenameProjectCommand(Guid ProjectId, string Name, Guid UserId) : IRequest<ProjectDto>;

public class RenameProjectCommandValidator : AbstractValidator<RenameProjectCommand>
{
    public RenameProjectCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}

public class RenameProjectCommandHandler : IRequestHandler<RenameProjectCommand, ProjectDto>
{
    private readonly IProjectRepository _projects;
    private readonly IFolderRepository _folders;
    private readonly IUnitOfWork _uow;

    public RenameProjectCommandHandler(IProjectRepository projects,
                                       IFolderRepository folders,
                                       IUnitOfWork uow)
    {
        _projects = projects;
        _folders  = folders;
        _uow      = uow;
    }

    public async Task<ProjectDto> Handle(RenameProjectCommand request, CancellationToken ct)
    {
        var project = await _projects.GetByIdAsync(request.ProjectId, ct)
            ?? throw new NotFoundException(nameof(Project), request.ProjectId);

        project.EnsureOwnedBy(request.UserId);
        project.Rename(request.Name);

        _projects.Update(project);
        await _uow.SaveChangesAsync(ct);

        var folderCount = (await _folders.GetByProjectAsync(project.Id, ct)).Count;

        return new ProjectDto(project.Id, project.Name, project.CreatedAt, project.UpdatedAt, folderCount);
    }
}

// ── Delete ───────────────────────────────────────────────────────
public record DeleteProjectCommand(Guid ProjectId, Guid UserId) : IRequest;

public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand>
{
    private readonly IProjectRepository _projects;
    private readonly IUnitOfWork _uow;

    public DeleteProjectCommandHandler(IProjectRepository projects, IUnitOfWork uow)
    {
        _projects = projects;
        _uow      = uow;
    }

    public async Task Handle(DeleteProjectCommand request, CancellationToken ct)
    {
        var project = await _projects.GetByIdAsync(request.ProjectId, ct)
            ?? throw new NotFoundException(nameof(Project), request.ProjectId);

        project.EnsureOwnedBy(request.UserId);

        _projects.Delete(project);
        await _uow.SaveChangesAsync(ct);
    }
}
