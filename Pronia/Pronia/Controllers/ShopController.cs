using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Models;
using Pronia.ViewModels;

namespace Pronia.Controllers
{
    public class ShopController:Controller
    {
        private readonly AppDbContext _contex;

        public ShopController(AppDbContext contex)
        {
            _contex = contex;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int? id)
        {
            if(id is null || id < 1)
            {
                return BadRequest();
            }
            
            Product? product = _contex.Products
                .Include(p=>p.ProductImages.OrderByDescending(pi=>pi.IsPrimary))
                .Include(p=>p.Category)
                .FirstOrDefault(p => p.Id == id);
            if (product is null)
            {
                return NotFound();
            }
            List<Product> releatedProducts = _contex.Products
                .Where(p=>p.CategoryId==product.CategoryId && p.Id!=id)
                .Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary!=null))
                .ToList();


            DetailVM detailVM = new() { 
            
            Product = product,
            RelatedProducts = releatedProducts,
            };

            return View(detailVM);
        }
    }
}
