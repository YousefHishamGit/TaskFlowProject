using System.Security.Claims;

namespace TaskFlow.Extensions
{
    public static class HttpContextLoggingExtensions
    {
        public static string GetLogUserId(this HttpContext context)
        {
            var userId = context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return string.IsNullOrEmpty(userId) ? "anonymous" : userId;
        }
    }
}
