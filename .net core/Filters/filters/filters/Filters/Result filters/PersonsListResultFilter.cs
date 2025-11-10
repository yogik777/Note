using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace filters.Filters.Result_filters
{
	public class PersonsListResultFilter : IAsyncResultFilter
	{
		private readonly ILogger<PersonsListResultFilter> _logger;
		public PersonsListResultFilter(
			ILogger<PersonsListResultFilter> logger)
		{
			_logger = logger;
		}
		public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
		{
			//TO DO: before logic
			_logger.LogInformation("{FilerName}.{MethodName} - before", nameof(PersonsListResultFilter),
				nameof(OnResultExecutionAsync));


			await next(); // call the subsequent filter [or] IActionResult
						  //TO DO: after logic 
			_logger.LogInformation("{FilerName}.{MethodName} - before", nameof(PersonsListResultFilter),
				nameof(OnResultExecutionAsync));

			//context.HttpContext.Response.Headers["last-Modified"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
		}
	}
}
