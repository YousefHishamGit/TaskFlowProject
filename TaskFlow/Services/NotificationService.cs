using Microsoft.AspNetCore.SignalR;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Tasks;
using TaskFlow.Hubs;

namespace TaskFlow.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationsHub> _hubContext;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IHubContext<NotificationsHub> hubContext,
            ILogger<NotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task TaskAssignedAsync(Guid assigneeId, TaskAssignedNotification notification, CancellationToken cancellationToken = default)
        {
            try
            {
                await _hubContext.Clients.User(assigneeId.ToString())
                    .SendAsync(NotificationEvents.TaskAssigned, notification, cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning(ex, "Failed to send TaskAssigned notification for Task ID: {TaskId}", notification.TaskId);
            }
        }

        public async Task TaskStatusChangedAsync(Guid projectId, TaskStatusChangedNotification notification, CancellationToken cancellationToken = default)
        {
            try
            {
                var groupName = NotificationEvents.ProjectGroup(projectId);
                await _hubContext.Clients.Group(groupName)
                    .SendAsync(NotificationEvents.TaskStatusChanged, notification, cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning(ex, "Failed to send TaskStatusChanged notification for Project ID: {ProjectId}", projectId);
            }
        }
    }
}
