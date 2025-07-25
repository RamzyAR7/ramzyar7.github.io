using Microsoft.AspNetCore.Mvc.Filters;

namespace PermissionAuth.ActionFilters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class LogScureInfoActionAttrbute : ActionFilterAttribute
    {
        private ILogger<LogScureInfoActionAttrbute> _logger;

        public LogScureInfoActionAttrbute(ILogger<LogScureInfoActionAttrbute> logger)
        {
            _logger = logger;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation("====>> Scure info fuck you!!!! <<====");
            await next();
        }
    }
}
