using AnsibeProject.Data;
using AnsibeProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AnsibeProject.Controllers
{
    public class AssignPController : Controller
    {
        UniversityContext _db;
        public AssignPController(UniversityContext db)
        {

            _db = db;
        }
        public IActionResult Index()
        {
            ViewBag.courses = _db.Courses.Where(C => C.CourseState == ActiveState.Active).ToList();
            return View(GetActiveProfessors());
        }

        public List<Professor> GetActiveProfessors()
        {
            List<Professor> temp = new List<Professor>();
            foreach (var p in _db.Professors.Include(p => p.Contract).ToList())
            {
                Professor tempProfessor = new Professor();
                if (tempProfessor.ActiveState == ActiveState.Active)
                {
                    tempProfessor.FileNumber = p.FileNumber;
                    tempProfessor.FirstName = p.FirstName;
                    tempProfessor.LastName = p.LastName;
                    tempProfessor.MiddleName = p.MiddleName;
                    tempProfessor.PhoneNumber = p.PhoneNumber;
                    tempProfessor.DateOfBirth = p.DateOfBirth;
                    tempProfessor.Contract = p.Contract;
                    tempProfessor.Speciality = p.Speciality;
                    tempProfessor.FullNameInArabic = p.FullNameInArabic;
                    tempProfessor.Email = p.Email;
                    tempProfessor.Rank = p.Rank;

                    temp.Add(tempProfessor);
                } 
                
            }
            return temp;
        }
    }
}
