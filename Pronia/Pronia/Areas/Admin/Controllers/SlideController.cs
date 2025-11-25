using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using NuGet.ContentModel;
using Pronia.DAL;
using Pronia.Models;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlideController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SlideController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;

            
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
            if(!slider.Photo.ContentType.Contains("image/"))
            {
                ModelState.AddModelError(nameof(Slider.Photo), "File type is incorrect");
                return View();
            }
            if (slider.Photo.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError(nameof(Slider.Photo), "File size is incorrect");
                return View();
            }
            //if (!ModelState.IsValid)
            //{
            //    return View();
            //}
            bool result = await _context.Sliders.AnyAsync(s => s.Order == slider.Order);
            if (result)
            {
                ModelState.AddModelError(nameof(Slider.Order), $"{slider.Order} order alredy exist");
                return View();
            }
            string path = Path.Combine(_env.WebRootPath, "assets", "images", "website - images", slider.Photo.FileName);
            

            FileStream stream = new FileStream(path,FileMode.Create);
            await slider.Photo.CopyToAsync(stream);

            slider.Image = slider.Photo.FileName;


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
        [HttpPost]
        public async Task<IActionResult> Update(int? id,Slider slide)
        {
            if (id is null || id < 1)
            {
                return BadRequest();
            }
            if(!ModelState.IsValid)
            {
                return View(slide);
            }
            bool result = await _context.Sliders.AnyAsync(s => s.Order == slide.Order && s.Id!=id);
            if (result)
            {
                ModelState.AddModelError(nameof(Slider.Order), $"{slide.Order} order alredy exist");
                return View(slide);
            }

            Slider existed = await _context.Sliders.FirstOrDefaultAsync(s => s.Id == id);
            if(existed is null)
            {
                return NotFound();
            }
            existed.Order = slide.Order;
            existed.Title = slide.Title;
            existed.Description = slide.Description;
            existed.SubTitle = slide.SubTitle;
            existed.Image = slide.Image;

            await _context.SaveChangesAsync();



            return RedirectToAction(nameof(Index));
        }
    }
}
