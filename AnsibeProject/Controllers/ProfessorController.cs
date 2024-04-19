using Microsoft.AspNetCore.Mvc;

namespace AnsibeProject.Controllers
{
    public class ProfessorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
