
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PermissionAuth.Exceptions;
using System.Net;

namespace PermissionAuth.MiddleWares
{
    // with constructor injection   
    public class GlobalExceptionHandlingMiddleware:IMiddleware
    {
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
        public GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                var requestId = Guid.NewGuid().ToString();
                context.Items["ReqId"] = requestId;
                context.Response.Headers.Add("X-Request-ID", requestId);


                _logger.LogInformation("========>  Request ID: {RequestId} - {Method} {Path}", requestId, context.Request.Method, context.Request.Path);
                await next(context);
                _logger.LogInformation("========>  Response ID: {RequestId} - {StatusCode}", requestId, context.Response.StatusCode);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex, context.Items["ReqId"]?.ToString() ?? "Unknown");
            }
        }

        private async Task HandleException(HttpContext context, Exception ex, string requestId)
        {
            _logger.LogError("========>  An error occurred while processing the request. {RequestId}: {errormsag}", requestId, ex.Message);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ex switch
            {
                ProductIsNotFoundException => (int)HttpStatusCode.NotFound,
                _ => (int)HttpStatusCode.InternalServerError
            };
            context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Title = "An error occurred while processing your request.",
                Detail = ex.Message,
                Status = context.Response.StatusCode
            });
        }
    }
}
