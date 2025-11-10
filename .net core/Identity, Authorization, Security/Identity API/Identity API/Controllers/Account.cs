using Microsoft.AspNetCore.Mvc;

namespace Identity_API.Controllers
{
    public class Account : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
