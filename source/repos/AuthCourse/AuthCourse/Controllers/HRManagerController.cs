using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthCourse.Controllers
{
    [Authorize(Policy = "ManagerHR")]
    public class HRManagerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
