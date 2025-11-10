using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace filters.Filters.Action_Filters
{
	public class PersonsListActionFilter : IActionFilter
	{
		private readonly ILogger<PersonsListActionFilter> _logger;
		public PersonsListActionFilter(ILogger<PersonsListActionFilter>logger)
		{
			_logger = logger;
		}
		public void OnActionExecuted(ActionExecutedContext context)
		{
			//context.HttpContext.Request.
			_logger.LogInformation("PersonListActionFilter.OnActionExecuted method");
		}

		public void OnActionExecuting(ActionExecutingContext context)
		{
			_logger.LogInformation("PersonsListActionFilter.OnActionExecuting method");

		}
	}
}
