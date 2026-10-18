using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Application.Features.Tasks;

namespace TaskFlow.Application.Common.Interfaces
{
    public interface INotificationService
    {
        Task TaskAssignedAsync(Guid assigneeId, TaskAssignedNotification notification, CancellationToken cancellationToken = default);

        Task TaskStatusChangedAsync(Guid projectId, TaskStatusChangedNotification notification, CancellationToken cancellationToken = default);
    }
}
