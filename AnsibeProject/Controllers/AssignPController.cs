using AnsibeProject.Data;
using AnsibeProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text.Json;
using static System.Collections.Specialized.BitVector32;

namespace AnsibeProject.Controllers
{
    public class AssignPController : Controller
    {
        private readonly UniversityContext _db;
        private const string SectionsSessionKey = "Sections";
        public AssignPController(UniversityContext db)
        {

            _db = db;
        }
        public IActionResult Index()
        {
            ViewBag.courses = _db.Courses.Where(C => C.CourseState == ActiveState.Active).ToList();
            ViewBag.Year = _db.Ansibes
                                           .Select(a => a.Year)
                                           .Distinct()
                                           .ToList();
            return View(GetActiveProfessors());
        }

        public List<Professor> GetActiveProfessors()
        {
            List<Professor> temp = _db.Professors.Where(p => p.ActiveState == ActiveState.Active).Include(p => p.Contract).ToList();

            return temp;
        }
        [HttpPost]
        public async Task<IActionResult> getAnsibeByYear([FromBody] JsonElement jsonElement)
        {
            if (jsonElement.TryGetProperty("year", out JsonElement yearElement))
            {
                string? year = yearElement.GetString();
                if (year == null) return BadRequest("Invalid year");
                var temp = await _db.Ansibes
                                        .Where(a => a.Year == year)
                                        .Select(a => new { a.Id, a.Year })
                                        .OrderByDescending(a => a.Id)
                                        .ToListAsync();
                return new JsonResult(temp);
            }
            return BadRequest("Invalid JSON data.");
        }


        [HttpPost]
        public async Task<IActionResult> getSectionOfTheAnsibeById([FromBody] KeyValuePairModel AnsibeId)
        {
            try
            {
                Ansibe? temp = await _db.Ansibes.Include(a => a.Sections)
                                                   .ThenInclude(s => s.Course)
                                                .Include(a => a.Sections)
                                                   .ThenInclude(s => s.Professor)
                                                .FirstOrDefaultAsync(a => a.Id == int.Parse(AnsibeId.Value));
                if (temp == null)
                {
                    return BadRequest($"No item found with this Id {AnsibeId}");
                }
                ICollection<Models.Section> mySections = temp.Sections;


                if (mySections == null || mySections.Count == 0)
                {
                    return BadRequest($"Ansibe found but its empty !!");
                }
                return PartialView("CourseSections", mySections);
            }catch(Exception ex)
            {
                return BadRequest("Invalid Data !");
            }
        }
        [HttpPost]
        public async Task<IActionResult> getSectionsOfById([FromBody] KeyValuePairModel AnsibeId)
        {
            try
            {
                Ansibe? temp = await _db.Ansibes.Include(a => a.Sections)
                                                   .ThenInclude(s => s.Course)
                                                .Include(a => a.Sections)
                                                   .ThenInclude(s => s.Professor)
                                                .FirstOrDefaultAsync(a => a.Id == int.Parse(AnsibeId.Value));
                if (temp == null)
                {
                    return BadRequest($"No item found with this Id {AnsibeId.Value}");
                }
                ICollection<Models.Section> mySections = temp.Sections;


                if (mySections == null || mySections.Count == 0)
                {
                    return BadRequest($"Ansibe found but its empty !!");
                }
                return Json(mySections);
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid Data !");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleView([FromBody] SectionsType sections)
        {
            if (sections == null) return BadRequest("Binding variable Failed !");
            if (sections.Sections == null || sections.Sections.Count == 0) return BadRequest("Sections are null or empty");
            ViewBag.professor = _db.Professors.Where(p => p.ActiveState == ActiveState.Active);

            if (sections.Type == "PS")
            {
                return PartialView("ProfessorSections", sections.Sections);
            }
            else
            {
                return PartialView("CourseSections", sections.Sections);
            }
        }
        [HttpPost]
        public  IActionResult SaveCreatedSections([FromBody]SectionsType sections)
        {
            //check databinding
            if (sections == null) return BadRequest();
            //check if created section is null or empty
            if (sections.TempSections == null || sections.TempSections.Count == 0) return BadRequest("Created Sections are null or empty");
            try
            {
                //foreach created section assign course and ensure data is correct and not manuplated
                foreach (var section in sections.TempSections)
                {

                    Course? course =  _db.Courses.SingleOrDefault(c => c.CourseCode == section.Course.CourseCode);
                    if (course == null) return BadRequest("Course Code does not exist !");

                    //one at least of Hourse , TP and TD should be have a value
                    if (section.CourseHours == null && section.TP == null && section.TD == null) return BadRequest("there is a section with TP & TD & course is null");

                    //ensure data in hours, TP and TD are correct
                    if (section.CourseHours != null) section.CourseHours = course.NumberOfHours;
                    if (section.TP != null) section.TP = course.TP;
                    if (section.TD != null) section.TD = course.TD;

                    section.Course = course;
                }

                // save new section in database
                 _db.AddRange(sections.TempSections);
                 _db.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            try
            {
                ViewBag.professor = _db.Professors.Where(p => p.ActiveState == ActiveState.Active);
                if (sections.Type == "CP")
                {
                    sections.Sections = sections.Sections ?? new List<Models.Section>();
                    sections.TempSections.AddRange(sections.Sections);
                    HttpContext.Session.SetString(SectionsSessionKey, JsonConvert.SerializeObject(sections.TempSections));
                    return PartialView("CourseSections", sections.TempSections);
                }
                else
                {
                    sections.Sections = sections.Sections ?? new List<Models.Section>();
                    sections.TempSections.AddRange(sections.Sections);
                    HttpContext.Session.SetString(SectionsSessionKey, JsonConvert.SerializeObject(sections.TempSections));
                    
                    return PartialView("ProfessorSections", sections.TempSections);
                }
            }catch(Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost]
        public IActionResult getCreatedSections()
        {
            var sectionsJson = HttpContext.Session.GetString(SectionsSessionKey);
            if (sectionsJson == null) return BadRequest("Should create section first");

            var sections = JsonConvert.DeserializeObject<List<Models.Section>>(sectionsJson);
            return Json(sections); ;

        }
    }
    
}
