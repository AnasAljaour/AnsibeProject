using Microsoft.AspNetCore.Mvc;

namespace AnsibeProject.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View( );
        }
    }
}
