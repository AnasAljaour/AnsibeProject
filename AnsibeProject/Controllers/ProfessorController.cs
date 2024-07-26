using AnsibeProject.Controllers.CourseWork;
using AnsibeProject.Data;
using AnsibeProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq.Expressions;

namespace AnsibeProject.Controllers
{
    public class ProfessorController : Controller
    {
        UniversityContext _db;
        public ProfessorController(UniversityContext db) { 
        
            _db = db;
                }
        public IActionResult Index()
        {
            return View(GetProfessors());
        }




        public IActionResult Edit(int? fileNumber)
        {
            if (fileNumber != null)
            {
                Professor? professorToUpdate;
                try
                {
                    professorToUpdate = getProfessorByFileNumber(fileNumber);

                    // if file number does not exist
                    if (professorToUpdate == null)
                    {
                        ModelState.AddModelError("", "The File Number does not exist.");
                    }
                    // id file number exist redirect to form
                    else
                    {
                        ViewBag.Action = "Edit";
                        ViewBag.ContractTypes = getContractTypes();
                        return View(professorToUpdate);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "an Error accourse");
                    ModelState.AddModelError("", ex.Message);
                }
            }
            // file number is null
            else
            {
                ModelState.AddModelError("", "File Number is null.");
            }
            // stay in the same page
            return RedirectToAction(nameof(Index));
        }



        //after submition the form from edit page
        [HttpPost]
        public IActionResult Edit(Professor professor)

        {
            try {
                if (professor != null) // instance of profeesor is null
                {
                    if (!professor.Contract.ContractType.IsNullOrEmpty())
                    {
                        Contract? contract = getContract(professor.Contract.ContractType);
                        professor.Contract = contract;
                    }

                    if (ModelState.IsValid)
                    {
                        try
                        {
                            // update the database and return to index file where list of professor exist
                            _db.Professors.Update(professor);
                            _db.SaveChanges();
                            return RedirectToAction(nameof(Index));
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", "failed to Edit Professor Information");
                            ModelState.AddModelError("", ex.Message);
                        }
                    }
                    else // Model not valid stay on the same page
                    {
                        ViewBag.Action = "Edit";
                        ViewBag.ContractTypes = getContractTypes();
                        return View(professor);
                    }
                }
                else // Professor is null
                {
                    ModelState.AddModelError("", "Professor instance is null");
                }
                // stay on the same Page
                ViewBag.Action = "Edit";
                ViewBag.ContractTypes = getContractTypes();
                return View(professor);

            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            }






        // go to form to enter the details about the new professor
        public IActionResult Add()
        {
            try
            {
                ViewBag.Action = "Add";
                ViewBag.ContractTypes = getContractTypes();
                return View();
            }catch(Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost]
        public IActionResult Add(Professor professor)
        {
            try
            {
                if (professor != null) // professor is not null
                {
                    if (!professor.Contract.ContractType.IsNullOrEmpty())
                    {
                        Contract? contract = getContract(professor.Contract.ContractType);
                        professor.Contract = contract;
                    }
                    if (ModelState.IsValid) // unvalide attributes were entered
                    {
                        try
                        {   // add the new professor to database and redirect to Index page
                            _db.Professors.Add(professor);
                            _db.SaveChanges();
                            return RedirectToAction(nameof(Index));
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", "failed to add Professor");
                            ModelState.AddModelError("", ex.Message);

                        }
                    }
                    else // Model is not valid
                    {
                        ViewBag.Action = "Add";
                        ViewBag.ContractTypes = getContractTypes();
                        return View(professor);
                    }
                }
                else // professore instance is null
                {
                    ModelState.AddModelError("", "Professore instance is null");
                }
                // go to form 
                ViewBag.Action = "Add";
                ViewBag.ContractTypes = getContractTypes();
                return View("Add", professor);
            }catch(Exception ex) { return BadRequest(ex.Message); }
        }




        public IActionResult Delete(int? fileNumber)
        {
            try
            {
                if (fileNumber != null)
                {
                    try
                    {   // call function delete 
                        Professor? p = _db.Professors.Include(p => p.Contract).FirstOrDefault(p => p.FileNumber == fileNumber);
                        if (p == null) ModelState.AddModelError("", "the professor does not exist.");
                        else
                        {
                            List<Models.Section> sections = _db.Sections.Include(p => p.Professor).Where(s => s.Professor != null && s.Professor.FileNumber == fileNumber).ToList();
                            if (sections.Count > 0)
                            {
                                foreach (var section in sections)
                                {
                                    section.Professor = null;
                                }
                                _db.UpdateRange(sections);
                            }
                            _db.Remove(p);
                            _db.SaveChanges();
                        }

                    }
                    // if any problem accoures
                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }
                }
                // file number is null
                else
                {
                    return BadRequest("File number is null");
                }

                return View("Index", GetProfessors());
            }catch(Exception ex) { return BadRequest(ex.Message); }
        }
        [HttpPost]
        public async Task<JsonResult> ChangeProfessorState(int FileNumber)
        {
            
                try
                {
                    Professor professor = await _db.Professors.SingleOrDefaultAsync(c => c.FileNumber == FileNumber);
                    if (professor == null) throw new Exception();
                    professor.ActiveState = (professor.ActiveState == ActiveState.Active) ? ActiveState.NotActive : ActiveState.Active;
                    _db.Professors.Update(professor);
                    await _db.SaveChangesAsync();
                    var additionalData = new
                    {
                        success = true,
                        target = professor.ActiveState.ToString()
                    };
                    return Json(additionalData);

                }
                catch
                {
                    var additionalData = new
                    {
                        success = false,
                        errorMessage = "Server Failed to Change the state of Professor"

                    };
                    return Json(additionalData);
                }
            

        } 


        // get a copy from all professor from database
        public List<Professor> GetProfessors()
        {
            try
            {
                List<Professor> temp = new List<Professor>();
                foreach (var p in _db.Professors.Include(p => p.Contract).ToList())
                {
                    Professor tempProfessor = new Professor();
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
                    tempProfessor.ActiveState = p.ActiveState;
                    tempProfessor.Rank = p.Rank;

                    temp.Add(tempProfessor);
                }
                return temp;
            }catch(Exception ex) { return new List<Professor>(); }
        }

        public Professor? getProfessorByFileNumber(int? fileNumber)
        {
            try
            {
                if (fileNumber == null) return null;
                Professor? professor = _db.Professors.Include(p => p.Contract)
                           .FirstOrDefault(p => p.FileNumber == fileNumber);

                return professor;
            }catch(Exception ex) { return null; }

        }
        public bool deleteProfessor(int? fileNumber)
        {
            try
            {


                Professor? professor = getProfessorByFileNumber(fileNumber);

                if (professor == null) return false;

                _db.Professors.Remove(professor);
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }


        }

        public List<string> getContractTypes()
        {
            try
            {
                return _db.Contracts.Select(c => c.ContractType).ToList();
            }catch (Exception ex) { return new List<string>(); }
        }

        public Contract? getContract(string ContractType)
        {
            try
            {
                return _db.Contracts.SingleOrDefault(c => c.ContractType == ContractType);
            }catch(Exception ex) { return null; }
        }
        public IActionResult ImportExcel(IFormFile file)
        {
            try
            {
                List<Professor> dataList;
                if (file != null && file.Length > 0)
                {
                    using (var stream = file.OpenReadStream())
                    {
                        var excelReader = new ExcelReader<Professor>(_db);
                        dataList = excelReader.ReadDataFromExcel(stream);
                        foreach (var c in dataList)
                        {
                            // Manual validation
                            var validationContext = new ValidationContext(c);
                            var validationResults = new List<ValidationResult>();

                            if (Validator.TryValidateObject(c, validationContext, validationResults, true))
                            {
                                try
                                {
                                    _db.Professors.Update(c);
                                    _db.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    _db.Professors.Add(c);
                                    _db.SaveChanges();
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


                return View("Index", GetProfessors());
            }catch(Exception ex) { return BadRequest(ex.Message); }
        }
    }
}
