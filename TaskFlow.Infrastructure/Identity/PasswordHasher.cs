using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Application.Common.Interfaces;

namespace TaskFlow.Infrastructure.Identity
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int workFactor = 12;
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor);//workFactor=> make Brute force hard
        }

        public bool Verify(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword); 
        }
    }
}
