using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Common.Interfaces
{
    public interface ICurrentUser
    {
        public Guid UserId { get; }
        public UserRole Role { get; }
    }
}
