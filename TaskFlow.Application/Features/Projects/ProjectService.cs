using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TaskFlow.Application.Common.Exceptions;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Common.Models;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Features.Projects
{
    public class ProjectService : IProjectService
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUser _CurUser;
        public ProjectService(IApplicationDbContext context,ICurrentUser CurUser)
        {
            _context = context;
            _CurUser = CurUser;

        }
        public async Task<ProjectDto> CreateAsync(CreateProjectRequest request, CancellationToken cancellationToken = default)
        {
            var newPro = new Project(request.Name, request.Description, _CurUser.UserId);
            _context.Projects.Add(newPro);
            await _context.SaveChangesAsync();
            return new ProjectDto
            (
              newPro.Id,
              newPro.Name,
              newPro.Description,
              newPro.OwnerId,
              newPro.Members.Count,
              newPro.CreatedAt,
             newPro.UpdatedAt)



            ;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var pro = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);
            if (pro is null)
            {
                throw new NotFoundException("Project not found");
            }
            if (_CurUser.Role != UserRole.Admin)
            {
                var isMember = await _context.ProjectMembers
                        .AnyAsync(m => m.ProjectId == id && m.UserId == _CurUser.UserId, cancellationToken);

                if (!isMember)
                {
                    throw new NotFoundException("Project not found");
                }
                if (pro.OwnerId != _CurUser.UserId)
                {
                    throw new ForbiddenException("You are not allowed to delete this project.");
                }
            }

            _context.Projects.Remove(pro);
            await _context.SaveChangesAsync();
        }
        public async Task<ProjectDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var pro = await _context.Projects
                            .Include(p => p.Members)
                            .SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
            if (pro is null) throw new NotFoundException($"This project by {id} is not Found ");
            return  ToDto(pro);

                
        }

        public async Task<PagedResult<ProjectDto>> GetMyProjectsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 50);

            var query = _context.Projects
                                .AsNoTracking()
                                .Where(p => p.Members.Any(m => m.UserId == _CurUser.UserId));
            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
        .OrderByDescending(p => p.CreatedAt)
        .ThenBy(p => p.Id)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(p => new ProjectDto(
            p.Id,
            p.Name,
            p.Description,
            p.OwnerId,
            p.Members.Count,
            p.CreatedAt,
            p.UpdatedAt
        ))
        .ToListAsync(cancellationToken);
            return new PagedResult<ProjectDto>(items, page, pageSize, totalCount);
        }

        public async Task<ProjectDto> UpdateAsync(Guid id, UpdateProjectRequest request, CancellationToken cancellationToken = default)
        {
            var proj =await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);
            if (proj is null)
            {
                throw new NotFoundException($"Project by Id = {id} not found");
            }

            var isMember = await _context.ProjectMembers
                .AnyAsync(m => m.ProjectId == id && m.UserId == _CurUser.UserId, cancellationToken);
            if (!isMember)
            {
                throw new NotFoundException("Project not found");
            }
            if (proj.OwnerId != _CurUser.UserId)
            {
                throw new ForbiddenException("You are not allowed to update this project.");
            }

            proj.Update(request.Name, request.Description);
            await _context.SaveChangesAsync(cancellationToken);
            var memberCount = await _context.ProjectMembers.CountAsync(m => m.ProjectId == id, cancellationToken);
            return new ProjectDto(
                            proj.Id,
                            proj.Name,
                            proj.Description,
                            proj.OwnerId,
                            memberCount,
                            proj.CreatedAt,
                            proj.UpdatedAt
            );
        }


        public async Task AddMemberAsync(Guid projectId, AddMemberRequest request, CancellationToken cancellationToken = default)
        {
            var currentUserId = _CurUser.UserId;
            if (currentUserId == null || currentUserId==Guid.Empty)
            {
                throw new UnauthorizedException("User is not authenticated.");
            }
            var project = await _context.Projects
                            .Include(p => p.Members)
                            .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);
            if (project is null)
            {
                throw new NotFoundException("Project not found.");
            }
            var isMember = await _context.ProjectMembers
                            .AnyAsync(pm => pm.ProjectId == projectId && pm.UserId == currentUserId, cancellationToken);
            if (!isMember)
            {
                throw new NotFoundException("Project not found.");
            }
            if (project.OwnerId != currentUserId)
            {
                throw new ForbiddenException("Only the project owner can add new members.");
            }
            var normalizedEmail = request.Email?.Trim().ToLower();

            Guid? targetUserId = await _context.Users
                                .Where(u => u.Email.ToLower() == normalizedEmail)
                                .Select(u => (Guid?)u.Id)
                                .FirstOrDefaultAsync();
            if (targetUserId == null)
            {
                throw new NotFoundException("User with the provided email was not found.");
            }
            project.AddMember(targetUserId.Value);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveMemberAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default)
        {
            var project = await _context.Projects
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);

            if (project == null)
                throw new NotFoundException("Project not found.");

            
            project.RemoveMember(userId);

         
            var tasksToUnassign = await _context.TaskItems
                .Where(t => t.ProjectId == projectId && t.AssigneeId == userId)
                .ToListAsync(cancellationToken);

            
            foreach (var task in tasksToUnassign)
            {
                task.Unassign(); 
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
        private static ProjectDto ToDto(Project project)
        {
            return new ProjectDto(
                project.Id,
                project.Name,
                project.Description,
                project.OwnerId,
                project.Members.Count, 
                project.CreatedAt,
                project.UpdatedAt
            );
        }
    }
}
