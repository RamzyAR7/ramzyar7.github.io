using Microsoft.AspNetCore.Mvc;

namespace AuthCourse.Controllers
{
    public class AccessDeniedController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
