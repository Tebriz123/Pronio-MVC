using Pronia.DAL;
using Pronia.Models;
using Pronia.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


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

       

            HomeVM homeVM = new HomeVM()
            {
                Sliders = _context
                .Sliders
                .OrderBy(s => s.Order)
                .Take(2)
                .ToList(),

                Products = _context
                .Products
                .OrderBy(p => p.CreatedAt)
                .Take(8)
                .Include(p => p.ProductImages.Where(pi => pi.IsPrimary != null))
                .ToList()

            };
            return View(homeVM);
        }
    }
}

//List<Slider> sliders = new List<Slider>()
//{
//    new Slider
//    {
//        Title = "Basliq 1",
//        SubTitle = "Komekci ",
//        Description ="Gullerden qalmadi",
//        CreatedAt = DateTime.Now,
//        Image="1-2-524x617.png",
//        IsDeleted = false,
//        Order=3
//    },
//    new Slider
//    {
//        Title = "Basliq 2",
//        SubTitle = "Komekci 2",
//        Description ="Sonanin Gulleri",
//        CreatedAt = DateTime.Now,
//        Image="red-flower.webp",
//        IsDeleted = false,
//        Order=1
//    },
//    new Slider
//    {
//        Title = "Basliq 3",
//        SubTitle = "Komekci 3",
//        Description ="Gelinbaciya gul",
//        CreatedAt = DateTime.Now,
//        Image="1-1-524x617.png",
//        IsDeleted = false,
//        Order=2
//    }
//};

//_context.Sliders.AddRange(sliders);
//_context.SaveChanges();