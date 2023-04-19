using Microsoft.AspNetCore.Mvc;

namespace ProjectCalendar.Middlewares
{
    public class LoggerMiddleware
    {
        private readonly RequestDelegate _next;
        public LoggerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILogger<LoggerMiddleware> logger)
        {
            logger.LogDebug(
              "Запрос {method} {url} ",
              context.Request?.Method,
              context.Request?.Path.Value);
            await _next.Invoke(context);
            logger.LogDebug("Ответ на запрос {method} {url}", 
                context.Request?.Method,
              context.Request?.Path.Value);
        }
    }
}
