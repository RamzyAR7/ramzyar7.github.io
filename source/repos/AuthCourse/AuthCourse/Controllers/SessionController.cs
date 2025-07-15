using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace AuthCourse.Controllers
{
    public class SessionController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SetSession(string Name, int Age)
        {
            // SetString(key, value)
            // key : Name, value: Name => key for getting the session value

            HttpContext.Session.SetString("Name", Name);
            HttpContext.Session.SetInt32("Age", Age);
            TempData["success"] = $"the session info {Name} and {Age} has been saved !!!";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult GetSession()
        {
            //GetString(key)
            // key : Name, value: Name => key for getting the session value

            string Name = HttpContext.Session.GetString("Name");
            int? Age = HttpContext.Session.GetInt32("Age");

            ViewBag.Name = Name ?? "No Name Found";
            ViewBag.Age = Age.HasValue ? Age.Value.ToString() : "No Age Found";

            return View();
        }
        [HttpGet]
        public IActionResult SetCookie(string Name, int Age)
        {
            // use cookie like dictionary
            HttpContext.Response.Cookies.Append("Name", Name, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(10) // this convert session cooki to persistent cookie
            });
            HttpContext.Response.Cookies.Append("Age", Age.ToString()); // session cookie

            return Content("Done");
        }
        public IActionResult GetCookie()
        {
            string Name = HttpContext.Request.Cookies["Name"];
            string Age = HttpContext.Request.Cookies["Age"];

            return Content($"Name: {Name ?? "No Name Found"}, Age: {Age ?? "No Age Found"}");
        }
    }
}
