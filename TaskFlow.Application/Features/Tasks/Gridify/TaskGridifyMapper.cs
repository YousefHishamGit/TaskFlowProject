using Gridify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Tasks.Gridify
{
    public static class TaskGridifyMapper
    {
        public static readonly IGridifyMapper<TaskItem> Instance = new GridifyMapper<TaskItem>(autoGenerateMappings: false)
            .AddMap("title", t => t.Title)
            .AddMap("status", t => t.Status)
            .AddMap("priority", t => t.Priority)
            .AddMap("dueDate", t => t.DueDate)
            .AddMap("projectId", t => t.ProjectId)
            .AddMap("createdAt", t => t.CreatedAt)
            .AddMap("assignee", t => t.Assignee != null ? t.Assignee.FullName : null);
    }
}
