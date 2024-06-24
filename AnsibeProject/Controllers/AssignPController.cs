using AnsibeProject.Data;
using AnsibeProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using static System.Collections.Specialized.BitVector32;

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
            List<Professor> temp = _db.Professors.Where(p => p.ActiveState == ActiveState.Active).Include(p => p.Contract).ToList();
           
            return temp;
        }
        public async Task<IActionResult> getAnsibeByYear(string year)
        {
            var temp = await _db.Ansibes
                                        .Where(a => a.Year == year)
                                        .Select(a => new { a.Id, a.Year })
                                        .ToListAsync();
            return new JsonResult(temp);
        }

        public async Task<IActionResult> getSectionOfTheAnsibeById(int AnsibeId)
        {

            Ansibe temp = await _db.Ansibes.Include(a=>a.Sections)
                                            .FirstOrDefaultAsync(a => a.Id == AnsibeId);
            if (temp == null)
            {
                return BadRequest($"No item found with this Id {AnsibeId}");
            }
            ICollection<Models.Section> mySections = temp.Sections;
            /*var customJson = mySections.Select(s => new
            {
                SectionIdentifier = s.SectionId,
                CourseCode = s.Course.CourseCode,
                CourseDescription = s.Course.CourseDescription,
                Credits = s.Course.NumberOfCredits,
                ProfessorName = s.Professor.Name,
                ProfessorTitle = s.Professor.Title,
                TeachingPeriod = s.TP,
                TeachingDays = s.TD,
                Hours = s.CourseHours,
                Language = s.Language
            });*/

            if (mySections==null || mySections.Count == 0)
            {
                return BadRequest($"Ansibe found but its empty !!");
            }
            return new JsonResult(mySections);
        }
     }
}
