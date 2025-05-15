using A_Service.Models;
using A_Service.ViewModels;
using B_EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Shopping.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _db;
        private readonly ILogger _logger;

        private ISession _session => HttpContext.Session;
        public AccountController(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
             ILoggerFactory loggerFactory

            )
        {
            _userManager = userManager;
            _signInManager = signInManager; 
            _db = db;
            _logger = loggerFactory.CreateLogger<AccountController>();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToLocal(returnUrl);
                //return RedirectToAction("Index", "Home");
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDTO model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user22 = await _signInManager.UserManager.Users
                   .SingleOrDefaultAsync(x =>
                    x.PhoneNumber == model.Username);

                if (user22 == null)
                {
                    ModelState.AddModelError("", "يوجد خطا فى كلمه المرور او الهاتف");

                    return View(model);
                }

                var result2 = await _signInManager.CheckPasswordSignInAsync(user22, model.Password, false);
                if (result2.Succeeded)
                {
                    var result = await _signInManager.PasswordSignInAsync(user22.UserName, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        var user = _db.Users.Where(u => u.UserName.ToLower() == model.Username.ToLower()).FirstOrDefault();



                        return RedirectToLocal(returnUrl);
                    }
                    ModelState.AddModelError("", "يوجد خطا فى كلمه المرور او الهاتف");
                }
                    //var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);               
                  
                return View(model);
            }
            else
            {
                ModelState.AddModelError("", "username or password is blank");
                return View(model);
            }
        }

        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Login");
        }
        #region Helpers
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
