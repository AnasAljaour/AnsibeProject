using AnsibeProject.Data;
using AnsibeProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System.ComponentModel.DataAnnotations;

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

        public IActionResult Import(IFormFile file)
        {
            var list = new List<Professor>();
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                //await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {

                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowcount = worksheet.Dimension.Rows;
                    var rank=Enum.GetValues(typeof(Models.Rank));
                    for (int row = 1; row <= rowcount; row++)
                    {
                        //Console.WriteLine(int.Parse(worksheet.Cells[row, 1].Value.ToString().Trim()));
                        list.Add(new Professor
                        {
                            
                            FullNameInArabic = worksheet.Cells[row, 1].Value.ToString().Trim(),
                            FirstName = worksheet.Cells[row, 2].Value.ToString().Trim(),
                            MiddleName = worksheet.Cells[row, 3].Value.ToString().Trim(),
                            LastName = worksheet.Cells[row, 4].Value.ToString().Trim(),
                            /*DateOfBirth = DateOnly.Parse(worksheet.Cells[row, 5].Value.ToString().Trim()),*/
                            Email = worksheet.Cells[row, 6].Value.ToString().Trim(),
                            PhoneNumber = worksheet.Cells[row, 7].Value.ToString().Trim(),
                            Speciality = worksheet.Cells[row, 8].Value.ToString().Trim(),
                            Rank = Enum.Parse<Models.Rank>(worksheet.Cells[row, 9].Value.ToString().Trim()),
                            Contract = _db.Contracts.Find(worksheet.Cells[row, 10].Value.ToString().Trim()),
                            FileNumber = int.Parse(worksheet.Cells[row, 11].Value.ToString().Trim())

                        });
                    }
                }
            }
           // _db.Professors.AddRange(list);
            return RedirectToAction(nameof(Index));
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

        }






        // go to form to enter the details about the new professor
        public IActionResult Add()
        {
            ViewBag.Action = "Add";
            ViewBag.ContractTypes = getContractTypes();
            return View();
        }

        [HttpPost]
        public IActionResult Add(Professor professor)
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
        }




        public IActionResult Delete(int? fileNumber)
        {
            if (fileNumber != null)
            {
                try
                {   // call function delete 
                    if (deleteProfessor(fileNumber)) return RedirectToAction("Index");
                    else
                    {
                        ModelState.AddModelError("", "Failed to delete");
                    }

                }
                // if any problem accoures
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error accoures while deleting the Professor");
                    ModelState.AddModelError("", ex.Message);
                }
            }
            // file number is null
            else
            {
                ModelState.AddModelError("", "File number is null");
            }

            return View("Index", GetProfessors());
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
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false, errorMessage = "Error occurred while changing state" });
            }

        } 


        // get a copy from all professor from database
        public List<Professor> GetProfessors()
        {
            List<Professor> temp = new List<Professor>();
            foreach(var p in _db.Professors.Include(p => p.Contract).ToList())
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
        }

        public Professor? getProfessorByFileNumber(int? fileNumber)
        {
            if (fileNumber == null) return null;
            Professor? professor = _db.Professors.Include(p => p.Contract) 
                       .FirstOrDefault(p => p.FileNumber == fileNumber);
                
            return professor;

        }
        public bool deleteProfessor(int? fileNumber)
        {
            Professor? professor = getProfessorByFileNumber(fileNumber);

            if (professor == null) return false;

            _db.Professors.Remove(professor);
            _db.SaveChanges();
            return true;



        }

        public List<string> getContractTypes()
        {
            return _db.Contracts.Select(c => c.ContractType).ToList();
        }

        public Contract? getContract(string ContractType)
        {
            return _db.Contracts.SingleOrDefault(c => c.ContractType == ContractType);
        }
    }
}
