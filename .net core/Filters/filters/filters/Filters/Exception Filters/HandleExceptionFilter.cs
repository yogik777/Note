using Microsoft.AspNetCore.Mvc.Filters;

namespace filters.Filters.Exception_Filters
{
	public class HandleExceptionFilter:IExceptionFilter
	{
		private readonly ILogger<HandleExceptionFilter> _logger;

		public HandleExceptionFilter(ILogger<HandleExceptionFilter> logger)
		{
			_logger = logger; 
		}
		//this method only executes when an exception is raised otherwase not 
		public void OnException(ExceptionContext context) 
		{
			_logger.LogError("Exception filter {FilterName}.{MethodName}\n{ExceptionType}\n{ExceptionMessage}"
				nameof(HandleExceptionFilter), 
				nameof(OnException), 
				context.Exception.GetType().ToString(),
				context.Exception.Message
				);
		}
	}
}
