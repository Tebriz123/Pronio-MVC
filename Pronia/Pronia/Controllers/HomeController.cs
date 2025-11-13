using Pronia.DAL;
using Pronia.Models;
using Pronia.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Pronia.DAL;
using Pronia.ViewModels;

namespace Backend_MVC_TASK_1.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Slider> slides = _context.Sliders.OrderBy(s => s.Order).Take(2).ToList();

            HomeVM homeVM = new HomeVM()
            {
                Sliders = slides

            };
            return View(homeVM);
        }
    }
}