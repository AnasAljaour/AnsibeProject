using AnsibeProject.Data;
using AnsibeProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace AnsibeProject.Controllers.CourseWork
{
    public class Courses
    {
        private readonly UniversityContext _universityContext;
        public Courses(UniversityContext universityContext)
        {
            _universityContext = universityContext;
        }

        public Course getCourseByCode(string CourseCode)
        {
            Course courseToreturn = _universityContext.Courses.Find(CourseCode);
            if (courseToreturn == null)
            {
                throw new Exception("Counrs not found !");
            }
            return courseToreturn;
        }
        public List<Course> getAllCourses()
        {
            return _universityContext.Courses.ToList();
        }
        public void AddCourse(Course course)
        {
            _universityContext.Courses.Add(course);
            _universityContext.SaveChanges();
        }
        public void UpdateCourseState(string courseCode, ActiveState newstate)
            {
            Course courseToUpdate = getCourseByCode(courseCode);
            courseToUpdate.CourseState = newstate;
            UpdateCourse(courseToUpdate);
        }
        public void UpdateCourse(Course course)
        {
            //Console.WriteLine("Courses.UpdateCourse");
            /*Course original = getCourseByCode(course.CourseCode);
            original.CourseDescription= course.CourseDescription;
            original.NumberOfCredits= course.NumberOfCredits;
            original.TotalNumberOfHours= course.TotalNumberOfHours;
            original.NumberOfHours= course.NumberOfHours;
            original.TP= course.TP;
            original.TD= course.TD;
            original.Semester= course.Semester;
            original.CourseState= course.CourseState;
            original.Major= course.Major;
            original.Obligatory= course.Obligatory;*/
            //_universityContext.Courses.Update(original);
            _universityContext.Entry(course).State = EntityState.Modified;
            _universityContext.SaveChanges();
        }
        public void DeleteCourse(String CourseCode)
        {

            _universityContext.Courses.Remove(getCourseByCode(CourseCode));
            _universityContext.SaveChanges();
        }
        /*
         * use this methods when we want to show the courses that are ready to bind with teacher
         */
        public List<Course> getActiveCourses()
        {
            return _universityContext.Courses.Where(x => x.CourseState == ActiveState.Active).ToList();
        }
        
        public List<Course> getCoursesOfMajor(CourseMajor major)
        {
            return _universityContext.Courses.Where(x => x.Major == major).ToList();
        }
        [HttpPost]
        public void ImportExcel(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var excelReader = new ExcelReader<Course>(_universityContext);
                    List<Course> dataList = excelReader.ReadDataFromExcel(stream);
                   foreach (var c in dataList)
                        {
                        // Manual validation
                        var validationContext = new ValidationContext(c);
                        var validationResults = new List<ValidationResult>();

                        if (Validator.TryValidateObject(c, validationContext, validationResults, true))
                        {
                            try
                            {
                               UpdateCourse(c);
                            }
                            catch (Exception ex)
                            {
                                AddCourse(c);
                            }
                        }
                    }
                    
                }
            }
            else
            {
                // Handle invalid file
                throw new Exception("Error while importing excel data...");
            }
        }
    }
}
