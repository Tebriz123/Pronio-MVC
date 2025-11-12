using Microsoft.AspNetCore.Mvc;

namespace Front_Project.Controllers
{
    public class DetailsController:Controller
    {
        public IActionResult Details()
        {
            return View();
        }
    }
}
