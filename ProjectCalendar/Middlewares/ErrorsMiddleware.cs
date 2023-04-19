namespace ProjectCalendar.Middlewares
{
    public class ErrorsMiddleware
    {
        private readonly RequestDelegate _next;
        public ErrorsMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context, ILogger<ErrorsMiddleware> logger)
        {
            //logger.LogDebug("Вход в метод ErrorsMiddleware.InvokeAsync");
            try
            {
                await _next.Invoke(context);
            }
            catch(Exception e)
            {
                logger.LogError("Исключение: {message}; Метод: {target}",e.Message,e.TargetSite);
            }
            //logger.LogDebug("Выход из метода ErrorsMiddleware.InvokeAsync");
        }
    }
}
