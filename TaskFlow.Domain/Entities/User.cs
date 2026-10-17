using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Domain.Common;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Exceptions;

namespace TaskFlow.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FullName { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;
        public UserRole Role { get; private set; }

        //Constructors => achieve Encapsulation

        public User(string fullName, string email, string passwordHash, UserRole role = UserRole.Member)
        {
            if (string.IsNullOrWhiteSpace(fullName)) {
                throw new DomainException("Full Name property is null or Empty");
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new DomainException("Email property is null or Empty");
            }
            if (string.IsNullOrWhiteSpace(passwordHash))
            {
                throw new DomainException("Password property is null or Empty");
            }

            Email = email.Trim().ToLowerInvariant();
            FullName = fullName.Trim();
            PasswordHash = passwordHash;
            Role = role;
        }

        private User() { }

    }
}
