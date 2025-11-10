using filters.AuthorizationFilter;
using filters.Filters.Action_Filters;
using filters.Filters.Result_filters;
using Microsoft.AspNetCore.Mvc;

namespace filters.Controllers
{
	[Route("{controller}/{action}")]
	public class HomeController : Controller
	{
		[Route("/")]
		//[TypeFilter(typeof(PersonsListActionFilter))]
		//[TypeFilter(typeof(PersonsListResultFilter))]
		[TypeFilter(typeof(TokenAuthorizationFilter))]
		public IActionResult Index()
		{
			return Ok("hello");
		}
	}
}
