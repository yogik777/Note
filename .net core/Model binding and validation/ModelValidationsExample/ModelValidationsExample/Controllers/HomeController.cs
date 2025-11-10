using Microsoft.AspNetCore.Mvc;
using ModelValidationsExample.CustomModelBinder;
using ModelValidationsExample.Models;

namespace ModelValidationsExample.Controllers
{
    public class HomeController : Controller
    {
        [Route("register")]
        public IActionResult Index([FromBody][ModelBinder(BinderType = typeof(PersonModelBinder))]Person person)
        {
            if (!ModelState.IsValid)
            {

                List<string> errorList =  ModelState.Values.SelectMany(value => value.Errors)
                    .Select(err => err.ErrorMessage).ToList();



                //List<string> errorsList = new List<string>();
                //foreach (var value in ModelState.Values)
                //{
                //    foreach (var error in value.Errors)
                //    {
                //        errorsList.Add(error.ErrorMessage);
                //    }
                //}

                //string errors =  string.Join("\n", errorsList);
                    
                return BadRequest(errorList);
            }
            return Content($"{person}");

        }
    }
}
