using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Application.Features.Tasks
{
    public record TaskStatusChangedNotification(
          Guid TaskId,
          string Title,
          Guid ProjectId,
          string OldStatus,
          string NewStatus,
          Guid ChangedById
      );
}
