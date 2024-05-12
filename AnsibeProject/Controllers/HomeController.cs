using AnsibeProject.Data;
using AnsibeProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
        public IActionResult AddSection(List<Section> mySections)
        {
            bool ok = true;
            foreach (Section section in mySections)
            {
                var validationContext = new ValidationContext(section);
                var validationResults = new List<ValidationResult>();

                if (!Validator.TryValidateObject(section, validationContext, validationResults, true))
                {
                    ok=false;
                    // If validation fails, add errors to the ModelState dictionary
                    foreach (var validationResult in validationResults)
                    {
                        foreach (var memberName in validationResult.MemberNames)
                        {
                            // Add each validation error to the ModelState dictionary
                            ViewData[section.SectionId.ToString()]= validationResult.ErrorMessage;
                        }
                    }
                }
            }
            if (!ok)
            {
                return View("CreateSection", mySections);
            }
            _db.Sections.AddRange(mySections);
            _db.SaveChanges();
            return View("CreateAnsibe",mySections);
        }
        

    }
}
