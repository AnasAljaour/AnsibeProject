using AnsibeProject.Data;
using AnsibeProject.Models;

namespace AnsibeProject.Controllers.CourseWork
{
    public class Courses
    {
        private readonly UniversityContext  universityContext;
        public Courses(UniversityContext universityContext)
        {
            universityContext = universityContext;
        }

        public Course getCourseByCode(string CourseCode) 
        {
            return universityContext.Courses.Find(CourseCode);  
        }
        public List<Course> getAllCourses() 
        {
            return universityContext.Courses.ToList();
        }
        public void AddCourse(Course course) 
        {
            universityContext.Courses.Add(course);
            universityContext.SaveChanges();
        }
        public void UpdateCourse(Course course)
        {
            universityContext.Courses.Update(course);
            universityContext.SaveChanges();
        }
        public void DeleteCourse(String CourseCode)
        {

            universityContext.Courses.Remove(universityContext.Courses.Find(CourseCode));
            universityContext.SaveChanges();
        }
        /*
         * use this methods when we want to show the courses that are ready to bind with teacher
         */
        public List<Course> getActiveCourses()
        {
            return universityContext.Courses.Where(x => x.CourseState == ActiveState.Active).ToList();
        }
        
        public List<Course> getCoursesOfMajor(CourseMajor major)
        {
            return universityContext.Courses.Where(x => x.Major == major).ToList();
        }
    }
}
