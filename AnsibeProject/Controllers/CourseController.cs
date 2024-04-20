using AnsibeProject.Controllers.CourseWork;
using AnsibeProject.Data;
using AnsibeProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AnsibeProject.Controllers
{
    public class CourseController : Controller
    {
        private readonly Courses courses;
        public CourseController(UniversityContext universityContext)
        {
            courses = new Courses(universityContext);
        }
        public IActionResult Index()
        {
            return View(courses.getAllCourses());
        }
        public IActionResult Edit(string CourseCode)
        {
            if (! CourseCode.IsNullOrEmpty()) 
            {
                Course courseToUpdate;
                try
                {
                    courseToUpdate = courses.getCourseByCode(CourseCode);
                    return View("Edit",courseToUpdate);
                }catch(Exception ex)
                {
                    ModelState.AddModelError("", "an error accourse while getting corse by code");
                    ModelState.AddModelError("",ex.Message);
                }
            }else
            {
                ModelState.AddModelError("", "failed to get courseCode String null or empty  issue");
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult Edit(Course courseToEdit)
        {
            if (courseToEdit != null)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        courses.UpdateCourse(courseToEdit);
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "failed to add Course");
                        ModelState.AddModelError("", ex.Message);
                    }
                }
                else
                {
                    return View("Edit", courseToEdit);
                }
            }
            else
            {
                ModelState.AddModelError("", "failed to get course instance null issue");
            }
            return View("Add", courseToEdit);
        }
        public IActionResult AddCourse()
        {
            return View("Add");
        }
        [HttpPost]
        public IActionResult AddCourse(Course courseToAdd) 
        {
            if (courseToAdd != null) 
            {
                if(ModelState.IsValid)
                {
                    try
                    {
                        courses.AddCourse(courseToAdd);
                        return RedirectToAction(nameof(Index));
                    }catch (Exception ex)
                    {
                        ModelState.AddModelError("", "failed to add Course");
                        ModelState.AddModelError("", ex.Message);
                    }
                }
                else
                {
                    return View("Add", courseToAdd);
                }
            }else
            {
                ModelState.AddModelError("", "failed to get course instance null issue");
            }
            return View("Add", courseToAdd);
        }
        public IActionResult Delete(string CourseCode) 
        {
            if(! CourseCode.IsNullOrEmpty())
            {
                try
                {
                    courses.DeleteCourse(CourseCode);

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error accoures while deleting the Course");
                    ModelState.AddModelError("", ex.Message);
                }
            }
            else
            {
                ModelState.AddModelError("", "failed to get course instance null or empty issue");
            }

            return RedirectToAction(nameof(Index));

        }
    }
}
