using System.Security.Claims;
using TaskFlow.Application.Common.Exceptions;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Api.Services
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var userIdClaim = user?.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    throw new UnauthorizedException("User is not authenticated or invalid user ID.");
                }
                return userId;
            }
        }

        public UserRole Role
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var roleClaim = user?.FindFirstValue(ClaimTypes.Role);

                if (string.IsNullOrEmpty(roleClaim) || !Enum.TryParse<UserRole>(roleClaim, true, out var role))
                {
                    throw new UnauthorizedException("User role is missing or invalid.");
                }
                return role;
            }
        }
    }
}