using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthCourse.Controllers
{
    [Authorize(Policy = "MustBelongToHR")]
    public class HumenResourceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
