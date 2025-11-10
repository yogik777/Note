using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using validation_practice.Models;

namespace validation_practice.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("validate")]
        public IActionResult CustomValidate(Person person)
        { 
            string message = ModelState.Values.SelectMany(value => value.Errors).Select(err => err.ErrorMessage).Aggregate((a, b)=> a + " ," + b);
            bool ans = ModelState.IsValid;
            return Ok(message);
        }
    }
}
