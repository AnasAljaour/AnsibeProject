using AnsibeProject.Data;
using AnsibeProject.Models;
using Azure.Core;
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

            return View();
        }
        [HttpPost]
        public IActionResult CreateNewAnsibe(string year)
        {
            ViewBag.courses = _db.Courses.Where(C => C.CourseState == ActiveState.Active).ToList();
            ViewBag.Year = _db.Ansibes
                                           .Select(a => a.Year)
                                           .Distinct()
                                           .ToList();

            Ansibe newAnsibe= new Ansibe();
            newAnsibe.Year = year;
            _db.Ansibes.Add(newAnsibe);
            _db.SaveChanges();
            ViewBag.AnsibeId=newAnsibe.Id;
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
        public async Task<IActionResult> getSectionOfTheAnsibeById([FromBody] AnsibeRequest request)
        {
            try
            {
                Ansibe? temp = await _db.Ansibes.Include(a => a.Sections)
                                                   .ThenInclude(s => s.Course)
                                                .Include(a => a.Sections)
                                                   .ThenInclude(s => s.Professor)
                                                .FirstOrDefaultAsync(a => a.Id == int.Parse(request.AnsibeId.Value));
                if (temp == null)
                {
                    return BadRequest($"No item found with this Id {request.AnsibeId}");
                }
                ICollection<Models.Section> mySections = temp.Sections;
                

                if (mySections == null || mySections.Count == 0)
                {
                    return BadRequest($"Ansibe found but its empty !!");
                }
                ICollection<Models.Section> myNewSections = CopySections(mySections);
                Ansibe? temp2 = await _db.Ansibes.Include(s=> s.Sections)
                                                .FirstOrDefaultAsync(a => a.Id == int.Parse(request.NewAnsibeId.Value));
                if (temp2 == null)
                {
                    return BadRequest("No item found with this Id {NewAnsibeId.Value}");
                }
                _db.Sections.RemoveRange(temp2.Sections);
                temp2.Sections = myNewSections;
                _db.Update(temp2);
                _db.SaveChanges();



                return PartialView("CourseSections", mySections);
                
            }catch(Exception ex)
            {
                return BadRequest("Invalid Data !");
            }
        }

        private ICollection<Models.Section> CopySections(ICollection<Models.Section> sectionsToCopy)
        {
            ICollection<Models.Section> newSections = new List<Models.Section>();
            foreach (var originalSection in sectionsToCopy)
            {
                Models.Section newSection = new Models.Section
                {
                    TP = originalSection.TP,
                    TD = originalSection.TD,
                    CourseHours = originalSection.CourseHours,
                    Language = originalSection.Language,

                    // Shallow copy of Course
                    Professor = originalSection.Professor,
                    Course = originalSection.Course
                    
                };
                _db.Entry(newSection.Course).State = EntityState.Unchanged;
                _db.Entry(newSection.Professor).State = EntityState.Unchanged;
                newSections.Add(newSection);
            }
            return newSections;
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
        public async Task<IActionResult> ToggleView([FromBody] AssignToggleRequest request)
        {
            if (request.AnsibeId == null || request.AnsibeId.Value == "") return BadRequest("Ansibe id is null or empty !");
            if (request.Allocation == null ) return BadRequest("Sections allocations are null or empty");
            ViewBag.professor = _db.Professors.Where(p => p.ActiveState == ActiveState.Active).Include(p => p.Contract);
            Ansibe? temp = await _db.Ansibes.Include(a => a.Sections)
                                                   .ThenInclude(s => s.Course)
                                                .Include(a => a.Sections)
                                                   .ThenInclude(s => s.Professor)
                                                .FirstOrDefaultAsync(a => a.Id == int.Parse(request.AnsibeId.Value));
            if (temp ==null)
            {
                return BadRequest("newAnsibe is null or empty !");
            }
            foreach (var pair in request.Allocation)
            {
              Models.Section s=  temp.Sections.FirstOrDefault(s => s.SectionId == int.Parse(pair.Key));
                if (s == null)
                {
                    return BadRequest("section is null or empty !");
                }
                Professor TempP = await _db.Professors.FirstOrDefaultAsync(p => p.FileNumber == int.Parse(pair.Value));
                if (TempP == null)
                {
                    return BadRequest("allocated prof is null");

                }
                s.Professor = TempP;
                _db.Update(s);

            }
            _db.Update(temp);
            _db.SaveChanges();
            if (request.Type == "PS")
            {
                return PartialView("ProfessorSections", temp.Sections);
            }
            else
            {
                return PartialView("CourseSections", temp.Sections);
            }
        }
        [HttpPost]
        public  IActionResult SaveCreatedSections([FromBody]SectionsType sections)
        {
            //check databinding
            if (sections == null) return BadRequest();

            Ansibe? temp =  _db.Ansibes.Include(a => a.Sections)
                                                   .ThenInclude(s => s.Course)
                                                .Include(a => a.Sections)
                                                   .ThenInclude(s => s.Professor)
                                                .FirstOrDefault(a => a.Id == int.Parse(sections.AnsibeId));
            if (temp == null)
            {
                return BadRequest("Ansibe is null or empty !");
            }

            //check if created section is null or empty
            if (sections.TempSections == null || sections.TempSections.Count == 0) return BadRequest("Created Sections are null or empty");
            try
            {
                //foreach created section assign course and ensure data is correct and not manuplated
                foreach (var worksection in sections.TempSections)
                {

                    Course? course =  _db.Courses.SingleOrDefault(c => c.CourseCode == worksection.Course.CourseCode);
                    if (course == null) return BadRequest("Course Code does not exist !");

                    //one at least of Hourse , TP and TD should be have a value
                    if (worksection.CourseHours == null && worksection.TP == null && worksection.TD == null) return BadRequest("there is a section with TP & TD & course is null");

                    //ensure data in hours, TP and TD are correct
                    if (worksection.CourseHours != null) worksection.CourseHours = course.NumberOfHours;
                    if (worksection.TP != null) worksection.TP = course.TP;
                    if (worksection.TD != null) worksection.TD = course.TD;

                    worksection.Course = course;
                    temp.Sections.Add(worksection);
                }

                // save new section in database
                //  _db.AddRange(sections.TempSections);
                _db.Update(temp);
                  _db.SaveChanges();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            try
            {
                ViewBag.professor = _db.Professors.Where(p => p.ActiveState == ActiveState.Active).Include(p => p.Contract);
                if (sections.Type == "" || sections.Type == "CP")
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
            }catch(Exception ex) { 
                return BadRequest(ex.Message); }
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
