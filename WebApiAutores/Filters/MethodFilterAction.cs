using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiAutores.Filters
{
    public class MethodFilterAction : IActionFilter
    {
        private readonly ILogger<MethodFilterAction> _logger;

        public MethodFilterAction(ILogger<MethodFilterAction> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("Antes de ejecutar los end points");
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("Despues de ejecutar los end points");
        }
    }
}
