using AnsibeProject.Controllers.ExcelWork;
using AnsibeProject.Data;
using AnsibeProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.Json.Nodes;
using System.Web;

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
            try
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
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
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
            }
            catch (Exception e)
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
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Download(int id)
        {
            try
            {

                if (id == null || id == 0) return BadRequest("Invalid Id");
                //if (!int.TryParse(id, out var ansibeId)) return;
                Ansibe? ansibe = await _db.Ansibes.Include(a => a.Sections)
                                                    .ThenInclude(s => s.Course)
                                                  .Include(a => a.Sections)
                                                  .ThenInclude(s => s.Professor)
                                                  .ThenInclude(p => p.Contract)
                    .SingleOrDefaultAsync(a => a.Id == id);
                if (ansibe == null) return BadRequest("ansibe not found");
                var stream= ExportAnsibeToExcel.ExportAnsibe(ansibe);


                return (File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Ansibe_"+ansibe.Year+".xlsx"));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
    }
}
