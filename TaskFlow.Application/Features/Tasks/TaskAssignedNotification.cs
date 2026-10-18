using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Application.Features.Tasks
{
    public record TaskAssignedNotification(
          Guid TaskId,
          string Title,
          Guid ProjectId,
          Guid AssignedById
      );
}
