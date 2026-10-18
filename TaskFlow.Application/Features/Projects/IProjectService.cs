using TaskFlow.Application.Common.Models;

namespace TaskFlow.Application.Features.Projects
{
    public interface IProjectService
    {
        Task<ProjectDto> CreateAsync(CreateProjectRequest request, CancellationToken cancellationToken = default);

        Task<ProjectDetailsDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<PagedResult<ProjectDto>> GetMyProjectsAsync(int page, int pageSize, CancellationToken cancellationToken = default);

        Task<ProjectDto> UpdateAsync(Guid id, UpdateProjectRequest request, CancellationToken cancellationToken = default);

        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);


        Task AddMemberAsync(Guid projectId, AddMemberRequest request, CancellationToken cancellationToken = default);
        Task RemoveMemberAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default);
    }
}