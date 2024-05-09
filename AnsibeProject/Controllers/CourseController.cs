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
            
            //check if the code is not empty
            if (! CourseCode.IsNullOrEmpty()) 
            {
                Course courseToUpdate;
                /*check if there is error while taking the course by its Code
                *Error => keep the user in the Index View and show the error
                *fine => get the course to be updated and fill the Edit view with it
                */
                try
                {
                    courseToUpdate = courses.getCourseByCode(CourseCode);
                    ViewBag.Action = "Edit";
                    return View("Edit",courseToUpdate);
                }catch(Exception ex)
                {
                    ModelState.AddModelError("Edit-Request", "an error accourse while getting corse by code");
                    ModelState.AddModelError("Edit-Request", ex.Message);
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
            //check the recived Object 
            if (courseToEdit != null)
            {
                //check for the annotations validity
                if (ModelState.IsValid)
                {
                    /*check if there is error while Updating the course 
                    *Error => keep the user in the Edit View and show the error
                    *fine => return the user to the Index View
                    */
                    try
                    {
                        courses.UpdateCourse(courseToEdit);
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("Edit", "failed to Edit Course");
                        ModelState.AddModelError("Edit", ex.Message);
                    }
                }
                else
                {
                    ViewBag.Action = "Edit";
                    return View("Edit", courseToEdit);
                }
            }
            else
            {
                ModelState.AddModelError("", "failed to get course instance null issue");
            }
            ViewBag.Action = "Edit";
            return View("Edit", courseToEdit);
        }
        public IActionResult Add()
        {
            ViewBag.Action = "Add";
            return View("Add");
        }
        [HttpPost]
        public IActionResult Add(Course courseToAdd) 
        {
            //check for the Object
            if (courseToAdd != null)
            {   //check for the 
                if (ModelState.IsValid)
                {
                    /*check if there is error while adding the course
                *Error => keep the user in the Add View and show the error
                *fine => insert the new Course then return the user to the Index View
                */
                    try
                    {
                        courses.AddCourse(courseToAdd);
                        return RedirectToAction(nameof(Index));
                    }catch (Exception ex)
                    {
                        ModelState.AddModelError("Add", "failed to add Course");
                        ModelState.AddModelError("Add", ex.Message);
                    }
                }
                else
                {
                    ViewBag.Action = "Add";
                    return View("Add", courseToAdd);
                }
            }else
            {
                //if the course recived is some how null
                ModelState.AddModelError("Add", "failed to get course instance null issue");
            }
            ViewBag.Action = "Add";
            return View("Add", courseToAdd);
        }
        public IActionResult Delete(string CourseCode) 
        {
            
            //check if not null 
            if(! CourseCode.IsNullOrEmpty())
            {
                /*check if there is error while taking the course by its Code or while deleting it
                *Error => keep the user in the Index View and show the error
                *fine => refresh the Index view with the new data
                */
                try
                {
                    courses.DeleteCourse(CourseCode);

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Delete-Request", "An error accoures while deleting the Course");
                    ModelState.AddModelError("Delete-Request", ex.Message);
                }
            }
            else
            {
                //if some how the code to be deleted is null or emty
                ModelState.AddModelError("Delete-Request", "failed to get course instance null or empty issue");
            }
            
            return RedirectToAction(nameof(Index));

        }
        public IActionResult ChangeState(Course course)
        {
            if(course!=null)
            {
                try
                {
                    courses.UpdateCourseState(course.CourseCode,course.CourseState);
                }catch (Exception ex)
                {
                    ModelState.AddModelError("ChangeState-Request", "changing course state failed !");
                    ModelState.AddModelError("ChangeState-Request", ex.Message);
                }
            }else
            {
                ModelState.AddModelError("ChangeState-Request", "course Code/course state failed with null or empty issue");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
