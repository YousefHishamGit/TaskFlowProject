using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Common.Models;
using TaskFlow.Application.Features.Projects;


namespace TaskFlow.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/projects")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpPost]
        public async Task<ActionResult<ProjectDto>> CreateAsync(
            [FromBody] CreateProjectRequest request,
            CancellationToken cancellationToken)
        {
            var project = await _projectService.CreateAsync(request, cancellationToken);


            return Created($"api/projects/{project.Id}", project);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<ProjectDto>>> GetMyProjectsAsync(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var result = await _projectService.GetMyProjectsAsync(page, pageSize, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProjectDto>> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            var project = await _projectService.GetByIdAsync(id, cancellationToken);
            return Ok(project);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ProjectDto>> UpdateAsync(
            Guid id,
            [FromBody] UpdateProjectRequest request,
            CancellationToken cancellationToken)
        {
            var project = await _projectService.UpdateAsync(id, request, cancellationToken);
            return Ok(project);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            await _projectService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }


        [HttpPost("{id:guid}/members")]
        public async Task<IActionResult> AddMember(Guid id, [FromBody] AddMemberRequest request, CancellationToken cancellationToken)
        {
            await _projectService.AddMemberAsync(id, request, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:guid}/members/{userId:guid}")]
        public async Task<IActionResult> RemoveMember(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            await _projectService.RemoveMemberAsync(id, userId, cancellationToken);
            return NoContent();
        }
    }
}