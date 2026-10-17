using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Common.Interfaces
{

    //main core of it is simplify the Unit Test
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Project> Projects { get; }
        DbSet<ProjectMember> ProjectMembers { get; }
        DbSet<TaskItem> TaskItems { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
