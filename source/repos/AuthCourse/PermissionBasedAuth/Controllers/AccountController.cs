using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PermissionBasedAuth.Entities;
using PermissionBasedAuth.Models;

namespace PermissionBasedAuth.Controllers
{
    public class AccountController(UserManager<ApplicationUser> _userManager, SignInManager<ApplicationUser> _signInManager) : Controller
    {
        public IActionResult Index()
        {
            return View(new LoginVM());
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                    return View("Index", model);
                }
                bool state = await _userManager.CheckPasswordAsync(user, model.Password);
                if (state)
                {
                    await _signInManager.SignInAsync(user, model.RememberMe);
                }
                else
                {

                    ModelState.AddModelError("", "Invalid username or password.");
                    return View("Index", model);
                }
                return RedirectToAction("Index", "Home");
            }
            return View("Index", model);
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            if (_signInManager.IsSignedIn(User) == true)
            {
                 await _signInManager.SignOutAsync();
                
                return RedirectToAction("Index", "Account");

            }
            return RedirectToAction("Index", "Home");
        }
    }
}
