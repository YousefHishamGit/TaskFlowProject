using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
namespace TaskFlow.Hubs
{
    [Authorize] // not any one can connect by Hub
    public class NotificationsHub:Hub
    {
        private readonly IApplicationDbContext _context;

        public NotificationsHub(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task JoinProject(Guid projectId, CancellationToken cancellationToken = default)
        {
            
            if (!Guid.TryParse(Context.UserIdentifier, out var userId))
            {
                throw new HubException("Invalid user identification.");
            }

           
            var isMember = await _context.ProjectMembers
                .AnyAsync(pm => pm.ProjectId == projectId && pm.UserId == userId, cancellationToken);

           
            if (!isMember)
            {
                throw new HubException("Project not found.");
            }

            var groupName = NotificationEvents.ProjectGroup(projectId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName, cancellationToken);
        }

        public async Task LeaveProject(Guid projectId, CancellationToken cancellationToken = default)
        {
            var groupName = NotificationEvents.ProjectGroup(projectId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName, cancellationToken);
        }
    }
}
