using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Application.Features.Tasks
{
    public record TaskDto(
          Guid Id,
          string Title,
          string? Description,
          string Status,
          int Priority,
          DateTime? DueDate,
          DateTime? CompletedAt,
          Guid ProjectId,
          Guid? AssigneeId,
          string? AssigneeName,
          Guid CreatedById,
          DateTime CreatedAt,
          DateTime? UpdatedAt
      );
}
