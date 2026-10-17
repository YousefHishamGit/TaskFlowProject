using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Domain.Common;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Exceptions;

namespace TaskFlow.Domain.Entities
{
    public class TaskItem : BaseEntity
    {
        public string Title { get; private set; } = null!;
        public string Description { get; private set; } = string.Empty;
        public TaskItemStatus Status { get; private set; } = TaskItemStatus.Todo;
        public int Priority { get; private set; }
        public DateTime? DueDate { get; private set; }
        public DateTime? CompletedAt { get; private set; } // assert when Status is Done
        //FK
        public Guid ProjectId { get; private set; }
        public Project Project { get; private set; } = null!;
    
        public Guid? AssigneeId { get; private set; } = null;
        public User? Assignee { get; private set; }
        public Guid CreatedById { get; private set; }
        public User CreatedBy { get; private set; } = null!;
        public TaskItem(string title, string? description, int priority, DateTime? dueDate, Guid projectId, Guid createdById)
        {
            if (string.IsNullOrWhiteSpace(title) || title.Trim().Length < 3)
            {
                throw new DomainException("Task title must be at least 3 characters long.");
            }


            if (priority < TaskPriority.MinPriority || priority > TaskPriority.MaxPriority)
            {
                throw new DomainException("Task priority is out of valid range.");
            }


            if (dueDate.HasValue && dueDate.Value < DateTime.UtcNow)
            {
                throw new DomainException("Due date cannot be in the past.");
            }

            if (projectId == Guid.Empty)
            {
                throw new DomainException("Project ID cannot be empty.");
            }
            if (createdById == Guid.Empty)
            {
                throw new DomainException("CreatedBy ID cannot be empty.");
            }
            AssigneeId = null;
            Title = title.Trim();
            Description = description != null ? description.Trim() : string.Empty;
            Priority = priority;
            DueDate = dueDate;
            ProjectId = projectId;
            CreatedById = createdById;
        }

        private TaskItem() { }




        public void Update(string title, string? desc, int priority, DateTime? dueDate) {
            if (string.IsNullOrWhiteSpace(title) || title.Trim().Length < 3)
            {
                throw new DomainException("Task title must be at least 3 characters long.");
            }
            if (priority < TaskPriority.MinPriority || priority > TaskPriority.MaxPriority)
            {
                throw new DomainException("Task priority is out of valid range.");
            }


            if (dueDate.HasValue && dueDate.Value != DueDate && dueDate.Value < DateTime.UtcNow)
            {
                throw new DomainException("Due date cannot be in the past.");
            }
            Title = title.Trim();
            Description = desc != null ? desc.Trim() : string.Empty;
            Priority = priority;
            DueDate= dueDate;
        }

        public void ChangeStatus(TaskItemStatus newStatus)
        {
            int difference = Math.Abs((int)Status - (int)newStatus);

            if (difference != 1)
            {
                throw new DomainException("Invalid Status Hierarchy transition.");
            }

            Status = newStatus;
            if (Status == TaskItemStatus.Done)
            {
                CompletedAt = DateTime.UtcNow;
            }
            else
            {
                CompletedAt = null;
            }
        }


        public void AssignTo(Guid userId)
        {
            if(userId==Guid.Empty)
            {
                throw new DomainException("User Id Require");

            }
            AssigneeId= userId;
            
        }


        public void Unassign()
        {
            
            AssigneeId = null;
            
        }
    }
}
    