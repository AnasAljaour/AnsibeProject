﻿using AnsibeProject.Controllers.CourseWork;
using AnsibeProject.Data;
using AnsibeProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

namespace AnsibeProject.Controllers
{
    public class CourseController : Controller
    {
        private readonly Courses courses;
        private readonly UniversityContext _db;
        public CourseController(UniversityContext universityContext)
        {
            courses = new Courses(universityContext);
            _db = universityContext;
        }
        public IActionResult Index()
        {
            try
            {
                return View(courses.getAllCourses());
            }catch(Exception e)
            {
                return BadRequest("ERROR !!! \n " + e.Message);

            }
        }
        public IActionResult Edit(string CourseCode)
        {

            //check if the code is not empty
            if (!CourseCode.IsNullOrEmpty())
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
                    return View("Edit", courseToUpdate);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Edit-Request", "an error accourse while getting corse by code");
                    ModelState.AddModelError("Edit-Request", ex.Message);
                }
            }
            else
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
                    }
                    catch (Exception ex)
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
            }
            else
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
            if (!CourseCode.IsNullOrEmpty())
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
        public async Task<JsonResult> ChangeCourseState(string CourseCode)
        {
            try
            {
                Course course = await _db.Courses.SingleOrDefaultAsync(c => c.CourseCode == CourseCode);
                if (course == null) throw new Exception();
                course.CourseState = (course.CourseState == ActiveState.Active) ? ActiveState.NotActive : ActiveState.Active;
                _db.Courses.Update(course);
                await _db.SaveChangesAsync();
                var additionalData = new
                {
                    success = true,
                    target = course.CourseState.ToString()
                };
                return Json(additionalData);
                
            }
            catch
            {
                var additionalData = new
                {
                    success = false,
                    errorMessage = "Server Failed to Change the state"
                };
                return Json(additionalData);

            }
            
        }

        public IActionResult ImportExcel(IFormFile file) 
        {
            List<Course> dataList;
            if (file != null && file.Length>0)
            {
                try
                {
                    courses.ImportExcel(file);
                }
                catch (Exception ex)
                {
                    return BadRequest("An error while Processing the data Error msg \n "+ex.Message);
                }
            }
            else
            {
                // Handle invalid file
                throw new Exception("Error while importing excel data...");
            }
            
               // ViewBag.ExcelData = dataList;


            return View("Index", courses.getAllCourses());
        }
    }

    }
