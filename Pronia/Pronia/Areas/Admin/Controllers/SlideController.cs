using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Pronia.DAL;
using Pronia.Models;
using System.Drawing;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlideController : Controller
    {
        private readonly AppDbContext _context;

        public SlideController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Slider> sliders =await _context.Sliders.ToListAsync();
            return View(sliders);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Slider slider)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
           bool result = await _context.Sliders.AnyAsync(s=>s.Order == slider.Order);
            if(result)
            {
                ModelState.AddModelError(nameof(Slider.Order), $"{slider.Order} order alredy exist");
                return View();
            }
                slider.CreatedAt = DateTime.Now;
            _context.Add(slider);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Update(int? id)
        {
            if(id is null || id < 1)
            {
                return BadRequest();
            }
            Slider existed = await _context.Sliders.FirstOrDefaultAsync(s=>s.Id == id);
            if(existed is null)
            {
                return NotFound();
            }

            return View(existed);
        }
    }
}
