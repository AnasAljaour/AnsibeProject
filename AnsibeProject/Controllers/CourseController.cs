using Microsoft.AspNetCore.Mvc;

namespace AnsibeProject.Controllers
{
    public class CourseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
