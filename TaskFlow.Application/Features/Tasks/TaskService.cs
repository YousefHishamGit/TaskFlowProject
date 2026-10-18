using FluentValidation.Results;
using Gridify;
using Gridify.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Application.Common;
using TaskFlow.Application.Common.Exceptions;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Common.Models;
using TaskFlow.Application.Features.Tasks.Gridify;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Exceptions;


namespace TaskFlow.Application.Features.Tasks
{
    public class TaskService : ITaskService
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUser _currentUser;
        private readonly ICacheService _cacheService;
        private readonly INotificationService _notificationService;

        public TaskService(IApplicationDbContext dbContext, ICurrentUser currentUser, ICacheService cacheService, INotificationService notificationService)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
            _cacheService = cacheService;
            _notificationService = notificationService;
        }

        public async Task<TaskDto> CreateAsync(Guid projectId, CreateTaskRequest request, CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUser.UserId;
            if (currentUserId == Guid.Empty||currentUserId==null) { throw new UnauthorizedException("User is not authenticated."); }
                 

            
            var isMemberOfProject = await _dbContext.Projects
                .AnyAsync(p => p.Id == projectId && p.Members.Any(m => m.UserId == currentUserId), cancellationToken);

            if (!isMemberOfProject)
            {
                throw new NotFoundException("Project not found.");
            }

            
            var taskItem = new TaskItem(
                request.Title,
                request.Description,
                request.Priority,
                request.DueDate,
                projectId,
                currentUserId
            );

           
            _dbContext.TaskItems.Add(taskItem);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var cacheKey = CacheKeys.GetProjectStatsKey(projectId);
            await _cacheService.RemoveAsync(cacheKey, cancellationToken);


            return MapToDto(taskItem);
        }

        public async Task<TaskDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUser.UserId;
            if (currentUserId == Guid.Empty || currentUserId == null) { throw new UnauthorizedException("User is not authenticated."); }
            var taskDto = await _dbContext.TaskItems
                .AsNoTracking()
                .Where(t => t.Id == id && t.Project.Members.Any(m => m.UserId == currentUserId))
                .Select(t => new TaskDto(
                    t.Id,
                    t.Title,
                    t.Description,
                    t.Status.ToString(),
                    t.Priority,
                    t.DueDate,
                    t.CompletedAt,
                    t.ProjectId,
                    t.AssigneeId,
                    t.Assignee != null ? t.Assignee.FullName : null,
                    t.CreatedById,
                    t.CreatedAt,
                    t.UpdatedAt
                ))
                .FirstOrDefaultAsync(cancellationToken);
            if (taskDto == null)
            {
                throw new NotFoundException("Task not found.");
            }

            return taskDto;
        }


        public async Task<PagedResult<TaskDto>> GetTasksAsync(GridifyQuery query, CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUser.UserId;
            if (currentUserId == Guid.Empty || currentUserId == null) { throw new UnauthorizedException("User is not authenticated."); }

            
            query.Page = query.Page < 1 ? 1 : query.Page;
            query.PageSize = Math.Clamp(query.PageSize == 0 ? 10 : query.PageSize, 1, 50);

            if (string.IsNullOrWhiteSpace(query.OrderBy))
            {
                query.OrderBy = "createdAt desc";
            }

           
            if (!query.IsValid(TaskGridifyMapper.Instance))
            {
                var failures = new List<ValidationFailure>
                {
                    new ValidationFailure("Query", "Invalid filter or orderBy parameters.")
                };
                throw new FluentValidation.ValidationException(failures);
            }

         
            var baseQuery = _dbContext.TaskItems
                .AsNoTracking()
                .Include(t => t.Assignee)
                .Where(t => t.Project.Members.Any(m => m.UserId == currentUserId));

           
            var pagedResult = await baseQuery.GridifyAsync(query, TaskGridifyMapper.Instance);

            
            var taskDtos = pagedResult.Data
                .Select(t => MapToDto(t, t.Assignee?.FullName))
                .ToList();

            return new PagedResult<TaskDto>(taskDtos, pagedResult.Count, query.Page, query.PageSize);
        }


        public async Task<TaskDto> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUser.UserId;
            if (currentUserId == Guid.Empty || currentUserId == null) { throw new UnauthorizedException("User is not authenticated."); }

            
            var taskItem = await _dbContext.TaskItems
                .Include(t => t.Assignee)
                .Where(t => t.Id == id && t.Project.Members.Any(m => m.UserId == currentUserId))
                .FirstOrDefaultAsync(cancellationToken);

            
            if (taskItem == null)
            {
                throw new NotFoundException("Task not found.");
            }
            taskItem.Update(
                request.Title,
                request.Description,
                request.Priority,
                request.DueDate
            );
            await _dbContext.SaveChangesAsync(cancellationToken);
            return MapToDto(taskItem, taskItem.Assignee?.FullName);
        }

        public async Task<TaskDto> ChangeStatusAsync(Guid id, ChangeTaskStatusRequest request, CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUser.UserId;
            if (currentUserId == Guid.Empty || currentUserId == null) { throw new UnauthorizedException("User is not authenticated."); }

            var taskItem = await _dbContext.TaskItems
                .Include(t => t.Assignee)
                .Include(t => t.Project) 
                .Where(t => t.Id == id && t.Project.Members.Any(m => m.UserId == currentUserId))
                .FirstOrDefaultAsync(cancellationToken);

            if (taskItem == null)
            {
                throw new NotFoundException("Task not found.");
            }

          
            var oldStatus = taskItem.Status.ToString();

            var newStatus = Enum.Parse<TaskItemStatus>(request.Status, ignoreCase: true);

            taskItem.ChangeStatus(newStatus);

           
            await _dbContext.SaveChangesAsync(cancellationToken);

            
            var cacheKey = CacheKeys.GetProjectStatsKey(taskItem.ProjectId);
            await _cacheService.RemoveAsync(cacheKey, cancellationToken);

            
            var notification = new TaskStatusChangedNotification(
                taskItem.Id,
                taskItem.Title,
                taskItem.ProjectId,
                oldStatus,
                taskItem.Status.ToString(),
                currentUserId
            );

            await _notificationService.TaskStatusChangedAsync(taskItem.ProjectId, notification, cancellationToken);

            return MapToDto(taskItem, taskItem.Assignee?.FullName);
        }
        public async Task<TaskDto> AssignAsync(Guid id, AssignTaskRequest request, CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUser.UserId;
            if (currentUserId == Guid.Empty || currentUserId == null) { throw new UnauthorizedException("User is not authenticated."); }

            var taskItem = await _dbContext.TaskItems
                 .Include(t => t.Assignee)
                 .Include(t => t.Project) 
                 .Where(t => t.Id == id && t.Project.Members.Any(m => m.UserId == currentUserId))
                 .FirstOrDefaultAsync(cancellationToken);

            if (taskItem == null)
            {
                throw new NotFoundException("Task not found.");
            }

           
            var oldAssigneeId = taskItem.AssigneeId;

            string? assigneeName = null;

            if (request.AssigneeId == null)
            {
                taskItem.Unassign();
            }
            else
            {
                var isAssigneeMember = await _dbContext.ProjectMembers
                    .AnyAsync(pm => pm.ProjectId == taskItem.ProjectId && pm.UserId == request.AssigneeId.Value, cancellationToken);

                if (!isAssigneeMember)
                {
                    throw new DomainException("Assignee must be a member of the project.");
                }

                taskItem.AssignTo(request.AssigneeId.Value);

                assigneeName = await _dbContext.Users
                    .Where(u => u.Id == request.AssigneeId.Value)
                    .Select(u => u.FullName)
                    .FirstOrDefaultAsync(cancellationToken);
            }

          
            await _dbContext.SaveChangesAsync(cancellationToken);

            
            if (request.AssigneeId.HasValue && request.AssigneeId.Value != oldAssigneeId && request.AssigneeId.Value != currentUserId)
            {
                var notification = new TaskAssignedNotification(
                    taskItem.Id,
                    taskItem.Title,
                    taskItem.ProjectId,
                    currentUserId
                );

                await _notificationService.TaskAssignedAsync(request.AssigneeId.Value, notification, cancellationToken);
            }

            return MapToDto(taskItem, assigneeName);
        }
        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUser.UserId;
            if (currentUserId == Guid.Empty || currentUserId == null) { throw new UnauthorizedException("User is not authenticated."); }

            var taskItem = await _dbContext.TaskItems
                .Where(t => t.Id == id && t.Project.Members.Any(m => m.UserId == currentUserId))
                .FirstOrDefaultAsync(cancellationToken);

          
            if (taskItem == null)
            {
                throw new NotFoundException("Task not found.");
            }

            if (taskItem.CreatedById != currentUserId)
            {
               
                var isOwner = await _dbContext.Projects
                    .AnyAsync(p => p.Id == taskItem.ProjectId && p.OwnerId == currentUserId, cancellationToken);

                if (!isOwner)
                {
                    throw new ForbiddenException("Only the task creator or project owner can delete this task.");
                }
            }

            _dbContext.TaskItems.Remove(taskItem);

            await _dbContext.SaveChangesAsync(cancellationToken);
            var cacheKey = CacheKeys.GetProjectStatsKey(taskItem.ProjectId);
            await _cacheService.RemoveAsync(cacheKey, cancellationToken);
        }


        private static TaskDto MapToDto(TaskItem task, string? assigneeName = null)
        {
            return new TaskDto(
                task.Id,
                task.Title,
                task.Description,
                task.Status.ToString(),
                task.Priority,
                task.DueDate,
                task.CompletedAt,
                task.ProjectId,
                task.AssigneeId,
                assigneeName,
                task.CreatedById,
                task.CreatedAt,
                task.UpdatedAt
            );
        }

    }
}
