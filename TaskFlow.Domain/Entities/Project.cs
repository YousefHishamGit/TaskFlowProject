using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Domain.Common;
using TaskFlow.Domain.Exceptions;

namespace TaskFlow.Domain.Entities
{
    public class Project : BaseEntity
    {
        public string Name { get; private set; } = null!;
        public string Description { get; private set; } = string.Empty;

        public Guid OwnerId { get; private set; }
        public User Owner { get; private set; } = null!;

        private readonly List<ProjectMember> _members = new();
        public IReadOnlyCollection<ProjectMember> Members => _members.AsReadOnly();

        public Project(string name, string? description, Guid ownerId)
        {
           
            if (ownerId == Guid.Empty)
            {
                throw new DomainException("Owner ID cannot be empty.");
            }

            
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new DomainException("Project name cannot be null or empty.");
            }

            Name = name.Trim();

            
            if (Name.Length < 3)
            {
                throw new DomainException("Project name must be at least 3 characters long.");
            }

           
            Description = description != null ? description.Trim() : string.Empty;

            OwnerId = ownerId;

            _members.Add(new ProjectMember(this.Id, ownerId));
        }

        private Project() { } 


        //Methods
        public void Update(string name, string? desc)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new DomainException("Project name cannot be null or empty.");
            }
            if (name.Length < 3)
            {
                throw new DomainException("Project name must be at least 3 characters long.");
            }

            Name = name.Trim();


           

            Description = desc != null ? desc.Trim() : string.Empty;

        }

        public void AddMember(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new DomainException("User ID cannot be empty.");
            }

            if (IsMember(userId))
            {
                throw new DomainException("User is already a member of this project.");
            }
            _members.Add(new ProjectMember(this.Id, userId));

        }
        public void RemoveMember(Guid userId) {

            if (userId == Guid.Empty)
            {
                throw new DomainException("User ID cannot be empty.");
            }
            if (OwnerId == userId)
            {
                throw new DomainException("Cannot remove the project owner from the project members.");
            }

            var member = _members.FirstOrDefault(m => m.UserId == userId);
            if (member==null)
            {
                throw new DomainException("User is already Not member of this project.");
            }
            _members.Remove(member);
        }

        public bool IsMember(Guid userId) { 
            return _members.Any(m => m.UserId == userId);
        
        }


    }
}
