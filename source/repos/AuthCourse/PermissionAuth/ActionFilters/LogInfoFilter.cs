using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace PermissionAuth.ActionFilters
{
    // global filter for logging action execution details
    public class LogInfoFilter : IAsyncActionFilter
    {
        private ILogger<LogInfoFilter> _logger;

        public LogInfoFilter(ILogger<LogInfoFilter> logger)
        {
            _logger = logger;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation("Executing action: {ActionName} with argument {Args} at {Time}", context.ActionDescriptor.DisplayName,JsonSerializer.Serialize(context.ActionArguments) , DateTime.UtcNow);
            await next();
            _logger.LogInformation("Executed action: {ActionName} at {Time}", context.ActionDescriptor.DisplayName, DateTime.UtcNow);
        }
    }
}
