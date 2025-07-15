using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using UserIdentity.Entities;
using UserIdentity.Models;

namespace UserIdentity.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHostEnvironment _hostEnvironment;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _hostEnvironment = hostEnvironment;
        }

        #region Register
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([FromForm]RegisterVM register)
        {

            var existingUserName = await _userManager.FindByNameAsync(register.UserName);
            if (existingUserName != null)
            {
                ModelState.AddModelError("UserName", "Username already exists.");
                return View("Register", register);
            }

            var existingUserEmail = await _userManager.FindByEmailAsync(register.Email);
            if (existingUserEmail != null)
            {
                ModelState.AddModelError("Email", "Email already exists.");
                return View("Register", register);
            }
            string fileName = String.Empty;
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            if (register.PhotoUrl != null && allowedExtensions.Contains(Path.GetExtension(register.PhotoUrl.FileName).ToLower()))
            {
                string UploadFolder = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot", "Photos");
                fileName = Guid.NewGuid().ToString() + "_" + register.PhotoUrl.FileName;

                var fullPath = Path.Combine(UploadFolder, fileName);

                using(var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await register.PhotoUrl.CopyToAsync(fileStream);
                }
                
            }
            else
            {
                ModelState.AddModelError("File", "Unsupported file type.");
                return View("Register", register);
            }

            ApplicationUser user = new ApplicationUser()
            {
                UserName = register.UserName,
                Email = register.Email,
                Address = register.Address ?? "none",   
                PhotoUrl = fileName?? "none"
            };
            if (!ModelState.IsValid)
            {
                return View("Register", register);
            }

            var states = await _userManager.CreateAsync(user, register.Password);

            if (states.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
            }
            else
            {
                foreach (var error in states.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View("Register", register);
            }

            return RedirectToAction("Index", "Home");
        }

        #endregion
     
        #region Login
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {

                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            var existedUser = await _userManager.FindByNameAsync(loginVM.UserName);

            if (existedUser != null)
            {
                bool found = await _userManager.CheckPasswordAsync(existedUser, loginVM.Password);

                if (found)
                {
                    var roles = await _userManager.GetRolesAsync(existedUser);
                    var claims = new Claim[]
                    {
                        new Claim(ClaimTypes.Role, roles.FirstOrDefault()) 
                    };
                    await _signInManager.SignInWithClaimsAsync(existedUser, loginVM.RememberMe, claims);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "The User Name or Password is Wrong.....");
                }
            }
            else
            {
                ModelState.AddModelError("", "The User Name or Password is Wrong.....");
            }
            return View(loginVM);
        }
        #endregion

        #region Logout
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            if(!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        #endregion

    }
}
