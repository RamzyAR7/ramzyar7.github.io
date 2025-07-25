
using System.Net;

namespace PermissionAuth.MiddleWares
{
    public class RateLimitingMiddleware : IMiddleware
    {
        private static int _counter = 0;
        private static DateTime _lastReqDateTime = DateTime.Now;
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            _counter++;
            if(DateTime.Now.Subtract(_lastReqDateTime).Seconds > 10)
            {
                _counter = 1;
                _lastReqDateTime = DateTime.Now;
                await next(context);
            }
            else
            {
                if (_counter > 5)
                {
                    _lastReqDateTime = DateTime.Now;
                    context.Response.Headers.Add("Retry-After", "10");
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    //context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    await context.Response.WriteAsync("Too many requests. Please try again later.");
                }
                else
                {
                    _lastReqDateTime = DateTime.Now;
                    await next(context);
                }
            }
        }
    }
}
