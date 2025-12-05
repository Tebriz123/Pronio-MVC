using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Models;
using Pronia.ViewModels;

namespace Pronia.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _singinManger;

        public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> singinManger)
        {
            _userManager = userManager;
            _singinManger = singinManger;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM userVM)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            AppUser user = new AppUser
            {
                UserName = userVM.UserName,
                Email = userVM.Email,
                Name = userVM.Name,
                Surname = userVM.Surname,

            };
         IdentityResult result = await _userManager.CreateAsync(user,userVM.Password);
            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            
            }
           await _singinManger.SignInAsync(user, false);
            return RedirectToAction("Index","Home");
        }

        public async Task<IActionResult> LogOut()
        {
           await _singinManger.SignOutAsync();
            return RedirectToAction("Index","Home");
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM userVM,string? returnUrl)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            AppUser? user = await _userManager.Users
                .FirstOrDefaultAsync(u=>u.UserName == userVM.UserNameOrEmail || u.Email == userVM.UserNameOrEmail);
            if(user is null)
            {
                ModelState.AddModelError(string.Empty, "User Information is wrong");
                return View();
            }
           var result = await _singinManger.PasswordSignInAsync(user,userVM.Password,userVM.IsPersistant,true);
            if(result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Your Account is Blocked Please Try Leter");
                return View();
            }

            if(!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "User Information is wrong");
                return View();
            }
            if(returnUrl is not null)
            {
            return Redirect(returnUrl);

            }
            return RedirectToAction("Index", "Home");
        }
    }
}
