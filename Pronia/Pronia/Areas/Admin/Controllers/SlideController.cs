using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using NuGet.ContentModel;
using Pronia.DAL;
using Pronia.Models;
using Pronia.Utilites.Extensions;
using Pronia.ViewModels;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlideController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SlideController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;


        }
        public async Task<IActionResult> Index()
        {
            List<GetSlideVM> slideVMs = await _context.Sliders
                .AsNoTracking()
                .Select(s => new GetSlideVM
                {
                    Id = s.Id,
                    Title = s.Title,
                    Image= s.Image,
                    Order = s.Order
                })
                .ToListAsync();

            return View(slideVMs);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSlideVM sliderVM)
        {
            if (!sliderVM.Photo.ValidateType("image/"))
            {
                ModelState.AddModelError(nameof(CreateSlideVM.Photo), "File type is incorrect");
                return View();
            }
            if (!sliderVM.Photo.ValidateSize(Utilites.Enums.FileSize.MB, 2))
            {
                ModelState.AddModelError(nameof(CreateSlideVM.Photo), "File size is incorrect");
                return View();
            }
            //if (!ModelState.IsValid)
            //{
            //    return View();
            //}
            bool result = await _context.Sliders.AnyAsync(s => s.Order == sliderVM.Order);
            if (result)
            {
                ModelState.AddModelError(nameof(CreateSlideVM.Order), $"{sliderVM.Order} order alredy exist");
                return View();
            }




            string fileName = await sliderVM.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");

            Slider slider = new Slider
            {
                Title = sliderVM.Title,
                SubTitle = sliderVM.SubTitle,
                Order = sliderVM.Order,
                Description = sliderVM.Description,
                Image = fileName,
                CreatedAt = DateTime.Now
            };



            _context.Add(slider);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Update(int? id)
        {
            if (id is null || id < 1)
            {
                return BadRequest();
            }
            Slider existed = await _context.Sliders.FirstOrDefaultAsync(s => s.Id == id);
            if (existed is null)
            {
                return NotFound();
            }
            UpdateSlideVM slideVM = new UpdateSlideVM
            {
                Title = existed.Title,
                SubTitle = existed.SubTitle,
                Order = existed.Order,
                Description = existed.Description,
                Image = existed.Image,

            };

            return View(slideVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateSlideVM slideVM)
        {
            if (id is null || id < 1)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return View(slideVM);
            }
            if (slideVM.Photo is not null)
            {
                if (!slideVM.Photo.ValidateType("image/"))
                {
                    ModelState.AddModelError(nameof(UpdateSlideVM.Photo), "File type is incorrect");
                    return View(slideVM);
                }
                if (!slideVM.Photo.ValidateSize(Utilites.Enums.FileSize.MB, 2))
                {
                    ModelState.AddModelError(nameof(UpdateSlideVM.Photo), "File size is incorrect");
                    return View(slideVM);
                }

            }
            bool result = await _context.Sliders.AnyAsync(s => s.Order == slideVM.Order && s.Id != id);
            if (result)
            {
                ModelState.AddModelError(nameof(Slider.Order), $"{slideVM.Order} order alredy exist");
                return View(slideVM);
            }

            Slider existed = await _context.Sliders.FirstOrDefaultAsync(s => s.Id == id);
            if (existed is null)
            {
                return NotFound();
            }

            if (slideVM.Photo is not null)
            {
                string fileName = await slideVM.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");
                existed.Image.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                existed.Image = fileName;
            }

            existed.Order = slideVM.Order;
            existed.Title = slideVM.Title;
            existed.Description = slideVM.Description;
            existed.SubTitle = slideVM.SubTitle;

            await _context.SaveChangesAsync();



            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id < 1)
            {
                return BadRequest();
            }
            Slider existed = await _context.Sliders.FirstOrDefaultAsync(s => s.Id == id);
            if (existed is null)
            {
                return NotFound();
            }
            existed.Image.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
            _context.Remove(existed);
            //existed.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
    }
}
