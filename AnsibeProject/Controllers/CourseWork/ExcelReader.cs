namespace AnsibeProject.Controllers.CourseWork;

using AnsibeProject.Data;
using AnsibeProject.Models;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text.RegularExpressions;

public class ExcelReader<T> where T:new()
{
    private readonly UniversityContext _universityContext;
    private static Dictionary<string, string> arabicToEnglish = new Dictionary<string, string>
        {
            {"أستاذ مساعد","AssistantProfessor" },
            {"أستاذ معيد","AssociateProfessor"},
            {"أستاذ","Professor" }

        };
   
    public ExcelReader(UniversityContext universityContext)
    {
        _universityContext = universityContext;
    }
    public List<T> ReadDataFromExcel(Stream stream)
    {
        List<T> dataList = new List<T>();

        using (var package = new ExcelPackage(stream))
        {
            var worksheet = package.Workbook.Worksheets[0]; // Assuming data is in the first sheet

            // Loop through the rows and columns to read data
            for (int row = 2; row <= worksheet.Dimension.Rows; row++)
            {
                T dataObject=new T();

                for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                {
                    try
                    {
                        var myAttribute = typeof(T).GetProperty(worksheet.Cells[1, col].Value?.ToString());
                        var cell = worksheet.Cells[row, col].Value?.ToString();

                        if (myAttribute != null && cell != null)
                        {
                            if (myAttribute.PropertyType.IsEnum)
                            {
                                Regex arabicRegex = new Regex(@"\p{IsArabic}");
                                if (arabicRegex.IsMatch(cell))
                                {

                                    if (arabicToEnglish.ContainsKey(cell))
                                    {
                                        cell = arabicToEnglish[cell];
                                    } else
                                    {
                                        continue;
                                    }

                                }
                                try {
                                    object myEnum = Enum.Parse(myAttribute.PropertyType, cell);
                                    myAttribute.SetValue(dataObject, myEnum);
                                }
                                catch (Exception ex) { }
                            } else if (myAttribute.PropertyType == typeof(Models.Contract))
                            {
                                try
                                {
                                    var contract = _universityContext.Contracts.FirstOrDefault(c => c.ContractType == cell);
                                    if (contract != null)
                                    {
                                        System.Diagnostics.Debug.WriteLine("Contract found: " + contract.ContractType);
                                        myAttribute.SetValue(dataObject, contract);
                                    }
                                    else
                                    {
                                        System.Diagnostics.Debug.WriteLine("Contract not found, setting default values.");
                                        myAttribute.SetValue(dataObject, new Models.Contract()
                                        {
                                            ContractType = "failed to load contract",
                                            MaxHours = 0,
                                            MinHours = 0,
                                            Professors = new List<Models.Professor>()
                                        });
                                    }
                                }
                                catch (Exception ex)
                                {
                                    myAttribute.SetValue(dataObject, new Models.Contract()
                                    {
                                        ContractType = "failed to load contract",
                                        MaxHours = 0,
                                        MinHours = 0,
                                        Professors = new List<Models.Professor>()
                                    });
                                }
                            } else if (myAttribute.PropertyType == typeof(DateOnly))
                            {
                                string dateFormat = "dd/mm/yyyy"; // Adjust the format according to your Excel file


                                /*myAttribute.SetValue(dataObject,DateOnly
                                    .FromDateTime(DateTime
                                    .FromOADate(Convert
                                    .ToDouble(cell))
                                    .Date));*/
                                try
                                {
                                    string[] dateTimeParts = cell.Split(' ');
                                    string datePart = dateTimeParts[0]; // Take the first part which should be the 
                                    // Try to convert the cell value to a double
                                    double oaDate = Convert.ToDouble(datePart);
                                    // Convert the OLE Automation date to DateOnly
                                    myAttribute.SetValue(dataObject, DateOnly.FromDateTime(DateTime.FromOADate(oaDate)));
                                }
                                catch (Exception e)
                                {
                                    myAttribute.SetValue(dataObject, DateOnly.MaxValue);
                                }
                            }
                            else
                            {
                                try
                                {
                                    myAttribute.SetValue(dataObject, Convert.ChangeType(cell, myAttribute.PropertyType));
                                } catch (Exception ex) { }
                            }


                        }
                        // Read cell value and add it to the list
                        //var cellValue = worksheet.Cells[row, col].Value?.ToString();
                        dataList.Add(dataObject);
                    }catch(Exception e)
                    {
                        continue;
                    }
                    }
                    
            }
        }

        return dataList;
    }
}
