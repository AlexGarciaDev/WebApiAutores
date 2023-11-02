using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiAutores.Filters
{
    public class MethodExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<MethodExceptionFilter> _logger;

        public MethodExceptionFilter(ILogger<MethodExceptionFilter> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, " Message de exception filter "+context.Exception.Message);

            base.OnException(context);
        }
    }
}
