using AnsibeProject.Data;
using AnsibeProject.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



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
            ViewBag.professor = _db.Professors.Where(p => p.ActiveState == ActiveState.Active).Include(p => p.Contract);
            return View();

        }

        public List<Professor> GetActiveProfessors()
        {
            List<Professor> temp = _db.Professors.Where(p => p.ActiveState == ActiveState.Active).Include(p => p.Contract).ToList();

            return temp;
        }
        [HttpPost]
        public IActionResult getAnsibeByYear([FromBody] KeyValuePairModel request)
        {
            try
            {
                if (request == null) return BadRequest("null request");
                if (request.Value == null || request.Value == "") return BadRequest("you must select a year");
                if (request.Key == null || request.Key == "") return BadRequest("there must be an Ansibe Created");
                if (!int.TryParse(request.Key, out var ansibeId))
                    return BadRequest("Invalide AnsibeId");
                var year = request.Value;
                if (year == null) return BadRequest("Invalid year");
                var temp = _db.Ansibes
                                        .Where(a => a.Year == year && a.Id != ansibeId)
                                        .Select(a => new { a.Id, a.Year })
                                        .OrderByDescending(a => a.Id)
                                        .ToList();
                return new JsonResult(temp);
            }
            catch (Exception e)
            {
                return BadRequest("Invalid JSON data.");
            }
        }


        [HttpPost]
        public IActionResult getSectionOfTheAnsibeById([FromBody] AnsibeRequest request)
        {
            try
            {
                Ansibe? temp = _db.Ansibes.Include(a => a.Sections)
                                                   .ThenInclude(s => s.Course)
                                                .Include(a => a.Sections)
                                                   .ThenInclude(s => s.Professor)
                                                .FirstOrDefault(a => a.Id == int.Parse(request.AnsibeId.Value));
                
                if (temp == null)
                {
                    return BadRequest($"No item found with this Id {request.AnsibeId}");
                }
               // await _db.Entry(temp).ReloadAsync();
                ICollection<Models.Section> mySections = temp.Sections;
                

                if (mySections == null || mySections.Count == 0)
                {
                    return BadRequest($"Ansibe found but its empty !!");
                }
                ICollection<Models.Section> myNewSections = CopySections(mySections);
                
                Ansibe? temp2 = _db.Ansibes.Include(s=> s.Sections)
                                                .FirstOrDefault(a => a.Id == int.Parse(request.NewAnsibeId.Value));
                if (temp2 == null)
                {
                    return BadRequest("No item found with this Id {NewAnsibeId.Value}");
                }
                _db.Sections.RemoveRange(temp2.Sections);
                temp2.Sections = myNewSections;
                _db.Update(temp2);
                _db.SaveChanges();

                ViewBag.professor = _db.Professors.Where(p => p.ActiveState == ActiveState.Active).Include(p => p.Contract);

                return PartialView("CourseSections", myNewSections);
                
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private ICollection<Models.Section> CopySections(ICollection<Models.Section> sectionsToCopy)
        {
            ICollection<Models.Section> newSections = new List<Models.Section>();
            foreach (var originalSection in sectionsToCopy)
            {
                Models.Section newSection = new Models.Section
                {
                    SectionId = 0,
                    TP = originalSection.TP,
                    TD = originalSection.TD,
                    CourseHours = originalSection.CourseHours,
                    Language = originalSection.Language,

                    // Shallow copy of Course
                    Professor =originalSection.Professor,
                    Course = originalSection.Course
                    
                };
                
                _db.Entry(newSection.Course).State = EntityState.Unchanged;
                if(newSection.Professor != null)
                {
                    _db.Entry(newSection.Professor).State = EntityState.Unchanged;
                    
                }
                
                newSections.Add(newSection);
            }
            
            return newSections;
        }
        /*
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
        */
        [HttpPost]
        public IActionResult ToggleView([FromBody] AssignRequest request)
        {
            if (request.AnsibeId == null || request.AnsibeId.Value == "") return BadRequest("Ansibe id is null or empty !");
            if (request.Allocation == null ) return BadRequest("Sections allocations are null or empty");
            ViewBag.professor = _db.Professors.Where(p => p.ActiveState == ActiveState.Active).Include(p => p.Contract);
            Ansibe? temp = _db.Ansibes.Include(a => a.Sections)
                                                   .ThenInclude(s => s.Course)
                                                .Include(a => a.Sections)
                                                   .ThenInclude(s => s.Professor)
                                                .FirstOrDefault(a => a.Id == int.Parse(request.AnsibeId.Value));
            if (temp ==null)
            {
                return BadRequest("newAnsibe is null or empty !");
            }
            foreach (var pair in request.Allocation)
            {
              Models.Section? s=  temp.Sections.FirstOrDefault(s => s.SectionId == int.Parse(pair.Key));
                if (s == null)
                {
                    return BadRequest("section is null or empty !");
                }
                Professor? TempP = _db.Professors.FirstOrDefault(p => p.FileNumber == int.Parse(pair.Value));
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

            
                ViewBag.professor = _db.Professors.Where(p => p.ActiveState == ActiveState.Active).Include(p => p.Contract);
                if (sections.Type == "" || sections.Type == "CP")
                {
                    
                   
                    return PartialView("CourseSections", temp.Sections);
                }
                else
                {
                    
        
                    return PartialView("ProfessorSections", temp.Sections);
                }
            }catch(Exception ex) { 
                return BadRequest(ex.Message); }
        }

        

        [HttpPost]
        public IActionResult DeleteSectionsOfAnsibe([FromBody] KeyValuePairModel AnsibeId)
        {
            try
            {
                if (AnsibeId.Value == null || AnsibeId.Value == "") return BadRequest("null or empty request");

                var ansibe = _db.Ansibes
                    .Include(a => a.Sections)
                    .SingleOrDefault(a => a.Id == int.Parse(AnsibeId.Value));
                if (ansibe == null) return BadRequest($"there is no Ansibe with ID {AnsibeId.Value}");
                if (ansibe.Sections == null) return BadRequest($"there is no sections to delete in Ansibe {AnsibeId.Value}");
                _db.Sections.RemoveRange(ansibe.Sections);
                _db.SaveChanges();
                return Json(new { success = true });

            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        public IActionResult SaveWork([FromBody] AssignRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("request failed");
                if (request.Allocation == null )
                    return BadRequest("Invalid allocation data!");

                if (request.AnsibeId == null)
                    return BadRequest("AnsibeId is null");
               
                if (!int.TryParse(request.AnsibeId.Value, out var ansibeId))
                    return BadRequest("Invalid AnsibeId!");

                
                var ansibe = _db.Ansibes
                                      .Include(a => a.Sections)
                                      .ThenInclude(s => s.Professor)
                                      .SingleOrDefault(a => a.Id == ansibeId);

                if (ansibe == null)
                    return BadRequest($"Ansibe {request.AnsibeId} not found");

                if (request.Allocation.Count > 0)
                {



                    foreach (var kvp in request.Allocation)
                    {
                        
                        if (!int.TryParse(kvp.Key, out var sectionId) || !int.TryParse(kvp.Value, out var professorId))
                            return BadRequest("Invalid SectionId or ProfessorId!");

                        
                        var section = ansibe.Sections.SingleOrDefault(s => s.SectionId == sectionId);
                        if (section == null)
                            return BadRequest($"Section with ID {kvp.Key} not exist in DB");

                        
                        var professor = _db.Professors.SingleOrDefault(p => p.FileNumber == professorId);
                        if (professor == null)
                            return BadRequest($"Professor with ID {kvp.Value} not found");

                        
                        section.Professor = professor;
                    }

                    
                    _db.Ansibes.Update(ansibe);
                    _db.SaveChanges();

                    
                }
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost]
        public IActionResult DeleteAssignement([FromBody] string sectionId)
        {
            try
            {
                if (sectionId == null || sectionId == "") BadRequest("section Id is null or empty");

                if(!int.TryParse(sectionId, out var sectionID))
                {
                    return BadRequest("Invalid section Id");
                }
                var section = _db.Sections.Include(s =>s.Professor).SingleOrDefault(s => s.SectionId == sectionID);
                if (section == null) return BadRequest("section id not exist");
                section.Professor = null;
                _db.Sections.Update(section);
                _db.SaveChanges();
                return Json(new { success = true });

            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost]
    public IActionResult DeleteSectionPermenetly([FromBody] string sectionId)
        {
            try
            {
                if (sectionId == null || sectionId == "") BadRequest("section Id is null or empty");

                if (!int.TryParse(sectionId, out var sectionID))
                {
                    return BadRequest("Invalid section Id");
                }
                var section = _db.Sections.SingleOrDefault(s => s.SectionId == sectionID);
                if (section == null) return BadRequest("section id not exist");
                _db.Remove(section);
                _db.SaveChanges();
                return Json(new { success = true });

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    
    
    }


    

    
    
}
