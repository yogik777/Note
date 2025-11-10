using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
	//[AllowAnonymous]
	[ApiController]
	[Route("{controller}/{action}")]
	public class PersonsController : ControllerBase
	{
		[Authorize]
		[HttpGet]
		public IActionResult GetPersons()
		{
			return Ok("Hello");
		}
	}
}
