using AnsibeProject.Data;
using AnsibeProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;


namespace AnsibeProject.Controllers
{
    public class HomeController : Controller

    {
        private readonly UniversityContext _db;
        public HomeController(UniversityContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {

            Ansibe? ansibeWithMaxId = _db.Ansibes
                                           .OrderByDescending(a => a.Id)
                                           .Include(a => a.Sections)
                                           .ThenInclude(s => s.Course)
                                           .Include(a => a.Sections)
                                           .ThenInclude(s => s.Professor)
                                           .ThenInclude(p => p.Contract)
                                           .FirstOrDefault();

            ViewBag.Year = _db.Ansibes
                                           .Select(a => a.Year)
                                           .Distinct()
                                           .ToList();

            return View(ansibeWithMaxId);
        }
        public IActionResult Details(int Id)
        {
            Ansibe? ansibes = _db.Ansibes
                                         .Include(a => a.Sections)
                                             .ThenInclude(s => s.Course)
                                         .Include(a => a.Sections)
                                             .ThenInclude(s => s.Professor)
                                         .FirstOrDefault(a => a.Id == Id);
            return View("Index", ansibes);
        }
        public IActionResult CreateAnsibe()
        {
            return View();
        }
        public IActionResult CreateSection()
        {
            List<Course> courses = _db.Courses.Where(c => c.CourseState == Models.ActiveState.Active).ToList();
            ViewBag.courses = courses;
            return View(courses);
        }
        [HttpPost]
        public IActionResult AddSection([FromBody] List<Section> mySections)
        {
            bool ok = true;
            foreach (Section section in mySections)
            {
                section.Course = _db.Courses.SingleOrDefault(c => c.CourseCode == section.Course.CourseCode);
                var validationContext = new ValidationContext(section);
                var validationResults = new List<ValidationResult>();

                if (!Validator.TryValidateObject(section, validationContext, validationResults, true))
                {
                    ok = false;
                    // If validation fails, add errors to the ModelState dictionary
                    foreach (var validationResult in validationResults)
                    {
                        foreach (var memberName in validationResult.MemberNames)
                        {
                            // Add each validation error to the ModelState dictionary
                            ViewData[section.SectionId.ToString()] = validationResult.ErrorMessage;
                        }
                    }
                }
            }
            if (!ok)
            {
                return View("CreateSection", mySections);
            }
            _db.Sections.AddRange(mySections);
            _db.SaveChanges();
            ViewBag.professors = _db.Professors.Where(p => p.ActiveState == ActiveState.Active).Include(p => p.Contract);
            return PartialView("AssignP", mySections);

        }
        [HttpPost]
        public IActionResult SaveAllocation([FromBody] List<KeyValuePairModel> mapData)
        {
            if (mapData.IsNullOrEmpty())
            {
                return BadRequest("empty or null data");
            }
            Ansibe allocation = new Ansibe();
            allocation.Id = 0;
            allocation.Year = DateTime.Now.Year.ToString() + "-" + (DateTime.Now.Year + 1).ToString();
            foreach (KeyValuePairModel kvp in mapData)
            {
                try
                {
                    Section section = _db.Sections.SingleOrDefault(s => s.SectionId == int.Parse(kvp.Key));
                    Professor professor = _db.Professors.SingleOrDefault(p => p.FileNumber == int.Parse(kvp.Value));


                    if (section == null)
                    {
                        return BadRequest($"Section with ID {kvp.Key} not found.");
                    }
                    if (professor == null)
                    {
                        return BadRequest($"Professor with file number {kvp.Value} not found.");
                    }

                    section.Professor = professor;
                    allocation.Sections.Add(section);
                    _db.Sections.Update(section);

                }
                catch (Exception ex)
                {
                    return BadRequest($"An error occurred: {ex.Message}");
                }
            }
            try
            {
                _db.Ansibes.Add(allocation);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }

            var responseData = new
            {
                success = true,
                id = allocation.Id
            };
            return Json(responseData);

        }



        // REAL WORK------------------------------------
        [HttpPost]
        public async Task<IActionResult> getAnsibeBasedOnSelectedYear([FromBody] KeyValuePairModel request)
        {
            try
            {
                if (request == null) return BadRequest("Invalide Request");
                if (request.Value == null || request.Value == "") return BadRequest("Empty or NULL Year");
                var temp = await _db.Ansibes
                                            .Where(a => a.Year == request.Value)
                                            .Select(a => new { a.Id, a.Year })
                                            .OrderByDescending(a => a.Id)
                                            .ToListAsync();
                return new JsonResult(temp);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }


        }
        [HttpPost]
        public async Task<IActionResult> Show([FromBody] string id)
        {
            try
            {

                if (id == "") return BadRequest("Ansibe Id is empty");
                if (!int.TryParse(id, out var ansibeId)) return BadRequest("Invalide Ansibe Id");
                Ansibe? ansibe = await _db.Ansibes.Include(a => a.Sections)
                                                    .ThenInclude(s => s.Course)
                                                  .Include(a => a.Sections)
                                                  .ThenInclude(s => s.Professor)
                                                  .ThenInclude(p => p.Contract)
                    .SingleOrDefaultAsync(a => a.Id == ansibeId);
                if (ansibe == null) return BadRequest("Ansibe does not exist!");
                return PartialView("Professores", ansibe);
            } catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Swap([FromBody] KeyValuePairModel request)
        {
            try
            {


                if (request.Value == null || request.Value == "") return BadRequest("the type is null or empty.");
                if (request.Key == null || request.Key == "") return BadRequest("Ansibe Id is null or empty.");
                if (!int.TryParse(request.Key, out var ansibeId)) return BadRequest("Invalide Ansibe Id.");
                Ansibe? ansibe = await _db.Ansibes.Include(a => a.Sections)
                                                        .ThenInclude(s => s.Course)
                                                      .Include(a => a.Sections)
                                                      .ThenInclude(s => s.Professor)
                                                      .ThenInclude(p => p.Contract)
                        .SingleOrDefaultAsync(a => a.Id == ansibeId);
                if (ansibe == null) return BadRequest("Ansibe does not exist!");
                if (request.Value == "C")
                {
                    return PartialView("Professores", ansibe);
                }
                if (request.Value == "P")
                {
                    return PartialView("Courses", ansibe);
                }
                return BadRequest("Type does not match");
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
    }
}
