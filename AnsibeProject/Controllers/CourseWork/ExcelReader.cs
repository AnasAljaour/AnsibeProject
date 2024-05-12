namespace AnsibeProject.Controllers.CourseWork;

using AnsibeProject.Data;
using AnsibeProject.Models;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Globalization;

public class ExcelReader<T> where T:new()
{
    private readonly UniversityContext _universityContext;
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
                    var myAttribute = typeof(T).GetProperty(worksheet.Cells[1, col].Value?.ToString());
                    var cell = worksheet.Cells[row, col].Value?.ToString();
                    if (myAttribute != null)
                    {
                        if (myAttribute.PropertyType.IsEnum)
                        {
                            try {
                                object myEnum = Enum.Parse(myAttribute.PropertyType, cell);
                                myAttribute.SetValue(dataObject, myEnum);
                            }
                            catch (Exception ex) { }
                        } else if (myAttribute.PropertyType == typeof(Contract))
                        {
                            try
                            {
                                var contract = _universityContext.Contracts.FirstOrDefault(c => c.ContractType == cell);
                                myAttribute.SetValue(dataObject, contract);
                            }
                            catch (Exception ex)
                            {
                                // IGNORE
                            }
                        }else if(myAttribute.PropertyType == typeof(DateOnly))
                        {
                            string dateFormat = "dd/mm/yyyy"; // Adjust the format according to your Excel file
                            
                            //DateOnly dateOnly = DateOnly.ParseExact(cell, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            //DateTime.FromOADate(Convert.ToDouble(cell));
                            myAttribute.SetValue(dataObject,DateOnly.FromDateTime(DateTime.FromOADate(Convert.ToDouble(worksheet.Cells[row, col].Value)).Date));
                            /*try
                            {
                                DateOnly dateOnly = DateOnly.ParseExact(cell, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                myAttribute.SetValue(dataObject, dateOnly);
                            }
                            catch(Exception e)
                            {
                                myAttribute.SetValue(dataObject, DateOnly.MaxValue);
                            }*/
                        }
                        else
                        {
                            try
                            {
                                myAttribute.SetValue(dataObject, Convert.ChangeType(cell, myAttribute.PropertyType));
                            }catch(Exception ex) { }
                        }
                        
                        
                    }
                    // Read cell value and add it to the list
                    //var cellValue = worksheet.Cells[row, col].Value?.ToString();
                    dataList.Add(dataObject);
                }
            }
        }

        return dataList;
    }
}
