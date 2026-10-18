using Serilog.Context;
using TaskFlow.Extensions;

namespace TaskFlow.Middleware
{
    public class LogContextMiddleware
    {
        private readonly RequestDelegate _next;

        public LogContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var userId = context.GetLogUserId();

            using (LogContext.PushProperty("UserId", userId))
            {
                await _next(context);
            }
        }
    }
}
