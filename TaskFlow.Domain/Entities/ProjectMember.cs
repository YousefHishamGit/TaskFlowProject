using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Domain.Common;
using TaskFlow.Domain.Exceptions;

namespace TaskFlow.Domain.Entities
{
    public class ProjectMember
    {
       

        public Guid ProjectId { get; private set; }
        public Project Project { get; private set; } = null!;
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        public DateTime JoinedAt { get; private set; } = DateTime.UtcNow;

        internal ProjectMember(Guid projectId, Guid userId)
        {
            if (projectId == Guid.Empty)
            {
                throw new DomainException("Project ID cannot be empty.");
            }

            if (userId == Guid.Empty)
            {
                throw new DomainException("User ID cannot be empty.");
            }
            ProjectId = projectId;
            UserId = userId;
        }
        private ProjectMember() { }
    }
}
