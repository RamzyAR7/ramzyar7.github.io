
using System.Diagnostics;

namespace PermissionAuth.MiddleWares
{
    public class ProfileMiddleware : IMiddleware
    {
        private readonly ILogger<ProfileMiddleware> _logger;

        public ProfileMiddleware(ILogger<ProfileMiddleware> logger)
        {
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var timer = new Stopwatch();
            timer.Start();
            _logger.LogInformation("========>  Request - {Method} {Path}", context.Request.Method, context.Request.Path);
            await next(context);
            timer.Stop();
            _logger.LogInformation("========>  Request - {Method} {Path} was completed in {ElapsedMilliseconds} ms", context.Request.Method, context.Request.Path, timer.ElapsedMilliseconds);
        }
    }
}
