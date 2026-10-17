using Gridify;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Common.Models;
using TaskFlow.Application.Features.Tasks;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpPost("projects/{projectId:guid}/tasks")]
    public async Task<ActionResult<TaskDto>> Create(
        Guid projectId,
        CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var task = await _taskService.CreateAsync(projectId, request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }
    [HttpGet("tasks")]
    public async Task<ActionResult<PagedResult<TaskDto>>> GetTasks(
            [FromQuery] GridifyQuery query,
            CancellationToken cancellationToken)
    {
        var result = await _taskService.GetTasksAsync(query);
        return Ok(result);
    }


    [HttpGet("tasks/{id:guid}")]
    public async Task<ActionResult<TaskDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var task = await _taskService.GetByIdAsync(id, cancellationToken);
        return Ok(task);
    }

  
    [HttpPut("tasks/{id:guid}")]
    public async Task<ActionResult<TaskDto>> Update(
        Guid id,
        UpdateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var task = await _taskService.UpdateAsync(id, request, cancellationToken);
        return Ok(task);
    }

    
    [HttpPatch("tasks/{id:guid}/status")]
    public async Task<ActionResult<TaskDto>> ChangeStatus(
        Guid id,
        ChangeTaskStatusRequest request,
        CancellationToken cancellationToken)
    {
        var task = await _taskService.ChangeStatusAsync(id, request, cancellationToken);
        return Ok(task);
    }

    [HttpPatch("tasks/{id:guid}/assignee")]
    public async Task<ActionResult<TaskDto>> Assign(
        Guid id,
        AssignTaskRequest request,
        CancellationToken cancellationToken)
    {
        var task = await _taskService.AssignAsync(id, request, cancellationToken);
        return Ok(task);
    }

  
    [HttpDelete("tasks/{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _taskService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}