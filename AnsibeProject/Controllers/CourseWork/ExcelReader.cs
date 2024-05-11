namespace AnsibeProject.Controllers.CourseWork;
using OfficeOpenXml;
using System;

public class ExcelReader<T> where T:new()
{
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
                        if(myAttribute.PropertyType.IsEnum)
                        {
                            try { 
                           object myEnum=Enum.Parse(myAttribute.PropertyType,cell);
                            myAttribute.SetValue(dataObject,myEnum);
                            }
                            catch (Exception ex) { }
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
