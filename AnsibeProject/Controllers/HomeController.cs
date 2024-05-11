using AnsibeProject.Data;
using AnsibeProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace AnsibeProject.Controllers
{
    public class HomeController : Controller

    {
        private readonly UniversityContext _db;
        public HomeController(UniversityContext db) {
            _db = db;
        }
        public IActionResult Index()
        {
            return View( );
        }
        public IActionResult CreateAnsibe()
        {
            return View();
        }
        public IActionResult CreateSection()
        {
           List<Course> courses = _db.Courses.Where(c => c.CourseState == Models.ActiveState.Active).ToList();
            ViewBag.courses = courses;
            return View(courses);
        }

    }
}
