using Microsoft.Extensions.Caching.Memory;

namespace PermissionAuth.MiddleWares
{
    public class RateLImitingV2Middleware: IMiddleware
    {
        private readonly IMemoryCache _memoryCache;
        private readonly int _maxRequests = 5;
        private readonly TimeSpan _timeWindow = TimeSpan.FromSeconds(10);

        public RateLImitingV2Middleware(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // Get IP Address (or use context.User.Identity.Name if authenticated)
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var cacheKey = $"RateLimit_{ipAddress}";

            var requestEntry = _memoryCache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _timeWindow;
                return new RequestCounter { Count = 0, FirstRequestTime = DateTime.UtcNow };
            });

            requestEntry.Count++;

            if (requestEntry.Count > _maxRequests)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.Headers["Retry-After"] = _timeWindow.TotalSeconds.ToString();
                await context.Response.WriteAsync("❌ Too many requests. Please try again later.");
            }
            else
            {
                _memoryCache.Set(cacheKey, requestEntry, _timeWindow);
                await next(context);
            }
        }

        private class RequestCounter
        {
            public int Count { get; set; }
            public DateTime FirstRequestTime { get; set; }
        }
        
    }
        
}


