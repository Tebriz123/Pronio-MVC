using Microsoft.AspNetCore.Mvc;

namespace Front_Project.Controllers
{
    public class HomeController:Controller
    {
       public IActionResult Index()
        {
            return View();
        }
    }
}
