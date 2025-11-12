using Microsoft.AspNetCore.Mvc;

namespace Front_Project.Controllers
{
    public class SearchController:Controller
    {
        public IActionResult Search()
        {
            return View();
        }
    }
}
