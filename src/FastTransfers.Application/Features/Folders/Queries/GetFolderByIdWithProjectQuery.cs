using FastTransfers.Application.DTOs;
using FastTransfers.Domain.Interfaces;
using MediatR;

namespace FastTransfers.Application.Features.Folders.Queries
{
    public record GetFolderByIdWithProjectQuery(Guid FolderId) : IRequest<FolderDto>;
    public class GetFolderByIdWithProjectHandler(
        IFolderRepository _folders,
        ISchemaTemplateRepository _schemas,
        IAppFileRepository _files
        ) : IRequestHandler<GetFolderByIdWithProjectQuery, FolderDto>
    {

        public async Task<FolderDto> Handle(GetFolderByIdWithProjectQuery request, CancellationToken ct)
        {
           
            var folder = await _folders.GetByIdWithProjectAsync(request.FolderId, ct);
            var schema = await _schemas.GetByFolderIdAsync(folder.Id, ct);
            var files = await _files.GetByFolderAsync(folder.Id, ct);

            var existingFolder = new FolderDto(
                folder.Id,
                folder.Name,
                folder.ProjectId,
                folder.CreatedAt,
                folder.UpdatedAt,
                schema is not null,
                files.Count
                );

            return existingFolder;
        }
    }


}
