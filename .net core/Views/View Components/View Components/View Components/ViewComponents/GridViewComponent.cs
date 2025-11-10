using Microsoft.AspNetCore.Mvc;
using View_Components.Models;

namespace View_Components.ViewComponents
{
    //[ViewComponent] this is optional if the class name is suffixed with View Component
    public class GridViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(PersonGridModel grid)
        {
            //PersonGridModel model = new PersonGridModel()
            //{
            //    GridTitle = "Persons List",
            //    Persons = new List<Person>()
            //    {
            //        new Person()
            //        {
            //            PersonName = "John",
            //            JobTitle = "Manager"
            //        },
            //        new Person()
            //        {
            //            PersonName = "Jones",
            //            JobTitle = "Asst. Manager"
            //        },
            //        new Person()
            //        {
            //            PersonName = "William",
            //            JobTitle = "Clerk"
            //        }
            //    }
            //};
            //ViewData["Grid"] = model;
            return View("Sample", grid);//invoked a partial view Views/Shared/Components/Grid/Default.cshtml
        }
    }
}
