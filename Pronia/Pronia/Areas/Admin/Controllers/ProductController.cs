using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using Pronia.DAL;
using Pronia.Models;
using Pronia.Utilites.Enums;
using Pronia.Utilites.Extensions;
using Pronia.ViewModels;
using System.Drawing;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            var productVMs = await _context.Products
                .Select(p => new GetAdminProduct
                {
                    Name = p.Name,
                    Id = p.Id,
                    Price = p.Price,
                    Image = p.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true).Image,
                    CategoryName = p.Category.Name
                })
                .ToListAsync();

            return View(productVMs);
        }

        public async Task<IActionResult> Create()
        {
            CreateProdutcVM productVM = new()
            {
                Categories = await _context.Categories.ToListAsync(),
                Tags = await _context.Tags.ToListAsync()
            };
            return View(productVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProdutcVM productVM)
        {

            productVM.Categories = await _context.Categories.ToListAsync();
            productVM.Tags = await _context.Tags.ToListAsync();
            
            if (!ModelState.IsValid)
            {

                return View(productVM);
            }

            if (!productVM.PrimaryPhoto.ValidateType("image/"))
            {
                ModelState.AddModelError(nameof(CreateProdutcVM.PrimaryPhoto), "File type is incorrect");
                return View(productVM);
            }
            if (!productVM.PrimaryPhoto.ValidateSize(FileSize.MB,1))
            {
                ModelState.AddModelError(nameof(CreateProdutcVM.PrimaryPhoto), "File size is incorrect");
                return View(productVM);
            }
            if (!productVM.SecondaryPhoto.ValidateType("image/"))
            {
                ModelState.AddModelError(nameof(CreateProdutcVM.SecondaryPhoto), "File type is incorrect");
                return View(productVM);
            }
            if (!productVM.SecondaryPhoto.ValidateSize(FileSize.MB, 1))
            {
                ModelState.AddModelError(nameof(CreateProdutcVM.SecondaryPhoto), "File size is incorrect");
                return View(productVM);
            }


            bool result = productVM.Categories.Any(c => c.Id == productVM.CategoryId);
            if (!result)
            {
                ModelState.AddModelError(nameof(CreateProdutcVM.CategoryId), "Category does not exists");
                return View(productVM);
            }
            if(productVM.TagIds is null)
            {
                productVM.TagIds = new();
            }

           productVM.TagIds =  productVM.TagIds.Distinct().ToList();

            bool tagResult = productVM.TagIds.Any(tId => !productVM.Tags.Exists(t => t.Id == tId));
            if(tagResult)
            {
                ModelState.AddModelError(nameof(CreateProdutcVM.TagIds), "Tags are wrong");
                return View(productVM);
            }

            


            bool resultName = await _context.Products.AnyAsync(p => p.Name == productVM.Name);
            if (resultName)
            {
                ModelState.AddModelError(nameof(CreateProdutcVM.Name), "Product name already exists");
                return View(productVM);
            }


            ProductImage main = new ProductImage
            {
                Image = await productVM.PrimaryPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                IsPrimary = true,
                CreatedAt = DateTime.Now
            };
            ProductImage secondary = new ProductImage
            {
                Image = await productVM.SecondaryPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                IsPrimary = false,
                CreatedAt = DateTime.Now
            };




            Product product = new()
            {
                Name = productVM.Name,
                SKU = productVM.SKU,
                Price = productVM.Price.Value,
                Description = productVM.Description,
                CategoryId = productVM.CategoryId.Value,
                CreatedAt = DateTime.Now,
                ProductTags = productVM.TagIds.Select(tId =>new ProductTag { TagId=tId}).ToList(),
                ProductImages = new()
                {
                    main,
                    secondary
                }
                
            };
            string message = string.Empty;

            foreach (IFormFile file in productVM.AdditionalPhotos)
            {
             
                if (file.ValidateType("image/"))
                {
                    
                    message += $"<p class=\"text-warning\">{file.Name} file type is inccorect</p>";
                    continue;
                }
                if (file.ValidateSize(FileSize.MB, 1))
                {
                    message += $"<p class=\"text-warning\">{file.Name} file size is inccorect</p>";
                    continue;
                }
                TempData["ImageWarning"] = message;

                product.ProductImages.Add(new()
                {
                    Image = await file.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                    IsPrimary = null,
                    CreatedAt = DateTime.Now

                });

            }
           



            //foreach (var tId in productVM.TagIds)
            //{
            //    product.ProductTags.Add(new ProductTag
            //    {
            //        TagId = tId
            //    });
            //}
            //productVM.Categories = await _context.Categories.ToListAsync();
            //productVM.Tags = await _context.Tags.ToListAsync();

            _context.Add(product);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id is null || id < 1)
            {
                return BadRequest();
            }
            Product? existed = await _context.Products
                .Include(p=>p.ProductImages)
                .Include(p=>p.ProductTags)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existed is null)
            {
                return NotFound();
            }

            UpdateProductVM productVM = new()
            {
                Name = existed.Name,
                SKU = existed.SKU,
                Description = existed.Description,
                CategoryId = existed.CategoryId,
                Price = existed.Price,
                TagIds = existed.ProductTags.Select(pt=>pt.TagId).ToList(),

                Categories = await _context.Categories.ToListAsync(),
                Tags = await _context.Tags.ToListAsync(),
                ProductImages = existed.ProductImages
            };
            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateProductVM productVM)
        {
            Product? existed = await _context.Products
              .Include(p => p.ProductImages)
              .Include(p => p.ProductTags)
              .Include(p => p.Category)
              .FirstOrDefaultAsync(p => p.Id == id);

            productVM.Categories = await _context.Categories.ToListAsync();
            productVM.Tags = await _context.Tags.ToListAsync();
            productVM.ProductImages = existed.ProductImages;

            if (!ModelState.IsValid)
            {
                return View(productVM);
            }
            if(productVM.PrimaryPhoto is not null)
            {

                if (!productVM.PrimaryPhoto.ValidateType("image/"))
                {
                    ModelState.AddModelError(nameof(UpdateProductVM.PrimaryPhoto), "File type is incorrect");
                    return View(productVM);
                }
                if (!productVM.PrimaryPhoto.ValidateSize(FileSize.MB, 1))
                {
                    ModelState.AddModelError(nameof(UpdateProductVM.PrimaryPhoto), "File size is incorrect");
                    return View(productVM);
                }
            }
            if (productVM.SecondaryPhoto is not null)
            {

                if (!productVM.SecondaryPhoto.ValidateType("image/"))
                {
                    ModelState.AddModelError(nameof(UpdateProductVM.SecondaryPhoto), "File type is incorrect");
                    return View(productVM);
                }
                if (!productVM.SecondaryPhoto.ValidateSize(FileSize.MB, 1))
                {
                    ModelState.AddModelError(nameof(UpdateProductVM.SecondaryPhoto), "File size is incorrect");
                    return View(productVM);
                }
            }
            bool result = productVM.Categories.Any(c => c.Id == productVM.CategoryId);
            if (!result)
            {
                ModelState.AddModelError(nameof(UpdateProductVM.CategoryId), "Category does not exists");
                return View(productVM);
            }
            if (productVM.TagIds is null)
            {
                productVM.TagIds = new();
            }

            productVM.TagIds = productVM.TagIds.Distinct().ToList();

            bool tagResult = productVM.TagIds.Any(tId => !productVM.Tags.Exists(t => t.Id == tId));
            if (tagResult)
            {
                ModelState.AddModelError(nameof(CreateProdutcVM.TagIds), "Tags are wrong");
                return View(productVM);
            }


            bool resultName = await _context.Products.AnyAsync(p => p.Name == productVM.Name && p.Id != id);
            if (resultName)
            {
                ModelState.AddModelError(nameof(UpdateProductVM.Name), "Product name already exists");
                return View(productVM);
            }


            if(productVM.PrimaryPhoto is not null)
            {
                string mainFileName = await productVM.PrimaryPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");
                ProductImage existedMain = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true);
                existedMain.Image.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");

                existed.ProductImages.Remove(existedMain);
                existed.ProductImages.Add(new()
                {
                    Image = mainFileName,
                    IsPrimary = true,
                    CreatedAt = DateTime.Now

                });
            }
            if (productVM.SecondaryPhoto is not null)
            {
                string secondaryFileName = await productVM.SecondaryPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");
                ProductImage existedSecondary = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == false);
                existedSecondary.Image.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");

                existed.ProductImages.Remove(existedSecondary);
                existed.ProductImages.Add(new()
                {
                    Image = secondaryFileName,
                    IsPrimary = false,
                    CreatedAt = DateTime.Now

                });
            }


            //_context.ProductTags.RemoveRange(existed.ProductTags
            //    .Where(pt => !productVM.TagIds
            //        .Exists(tId => pt.TagId == tId))
            //    .ToList());



            //_context.ProductTags
            //    .AddRange(productVM.TagIds
            //        .Where(tId => !existed.ProductTags
            //            .Exists(pt => pt.TagId == tId))
            //        .Select(tId => new ProductTag { TagId = tId })
            //        .ToList());

            existed.ProductTags = new List<ProductTag>();

            productVM.TagIds.ForEach(tId => {
                existed.ProductTags.Add(new ProductTag()
                {
                    ProductId = existed.Id,
                    TagId = tId
                });
            });


            existed.Name = productVM.Name;
            existed.SKU = productVM.SKU;
            existed.Description = productVM.Description;
            existed.Price = productVM.Price.Value;
            existed.CategoryId = productVM.CategoryId.Value;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

    }
}
