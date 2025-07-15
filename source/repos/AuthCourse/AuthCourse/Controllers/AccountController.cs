using AuthCourse.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthCourse.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(Credential credential)
        {
            if (!ModelState.IsValid) return View();

            if (!IsAuthentecated(credential.Username, credential.Password)) return View();

            var Claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, credential.Username),
                new Claim(ClaimTypes.Email, "Admin123@gmail.com"),
                new Claim("Department", "HR"),
                new Claim("Admin", "True")
            };

            var Idetity = new ClaimsIdentity(Claims, "MyCookies");


            var Principal = new ClaimsPrincipal(Idetity); // provide p.k identity


            
            /*
             * Ensures the authentication cookie behaves according to the user's session preference:
             * - If IsPersistent = false, the cookie is a session cookie and will be removed when the browser is closed, regardless of the expiration time.
             * - If IsPersistent = true, the cookie persists even after the browser is closed, until it expires or the user logs out.
             * This logic helps enforce security by allowing users to choose whether to stay signed in across browser sessions.
             */

            await HttpContext.SignInAsync(Principal, new AuthenticationProperties
            {
                IsPersistent = credential.RememberMe
            });

            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        private bool IsAuthentecated(string username, string password)
        {
            if ((username == null || username != "Admin") || (password == null || password != "123"))
            {
                return false;
            }
            return true;
        }

    }
}
