using Microsoft.AspNetCore.Mvc.Filters;

namespace filters.Filters.ResourceFilter
{

	public class FeatureDisabledResourceFilter : IAsyncResourceFilter
	{
		public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
		{
			//TO DO: before logic
			await next();
			//TO DO: after logic 
		}
	}
}
