using Gridify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Application.Common.Models;

namespace TaskFlow.Application.Features.Tasks
{
    public interface ITaskService
    {
        Task<TaskDto> CreateAsync(Guid projectId, CreateTaskRequest request, CancellationToken cancellationToken = default);
        Task<TaskDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        //GridifyQuery => default has 4 properties (PageSize,page,orderBy,Filter)
        /*
         * class TaskGridifyMapper => for Filter and sort 
         */
        Task<PagedResult<TaskDto>> GetTasksAsync(GridifyQuery query, CancellationToken cancellationToken = default);
        Task<TaskDto> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken = default);
        Task<TaskDto> ChangeStatusAsync(Guid id, ChangeTaskStatusRequest request, CancellationToken cancellationToken = default);
        Task<TaskDto> AssignAsync(Guid id, AssignTaskRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
