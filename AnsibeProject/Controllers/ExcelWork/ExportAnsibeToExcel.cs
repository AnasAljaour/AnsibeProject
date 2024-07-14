using AnsibeProject.Models;
using Azure;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http.HttpResults;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using System;
using System.Linq;
namespace AnsibeProject.Controllers.ExcelWork
{
    public class ExportAnsibeToExcel
    {
        private static  Dictionary<string, List<int>> myMap = new Dictionary<string, List<int>>();
        private static Dictionary<int, string> labeledSections = new Dictionary<int, string>();
        private static Dictionary<string,int> contractMapSheet2=new Dictionary<string, int> 
        {
            {"ملاك",6 },
            {"ساعة",4 },
            {"تفرغ",5 }
        };
        private static Dictionary<string, int> contractMapSheet1 = new Dictionary<string, int>
        {
            {"ملاك",3 },
            {"ساعة",5 },
            {"تفرغ",4 }
        };

        private static int Color=0;

        private static XLColor[] groupColors = {

            XLColor.Gray,
            XLColor.LightGray,
            XLColor.DimGray
            //XLColor.DarkSlateGray,
            //XLColor.LightSlateGray,
            //XLColor.GrayAsparagus,
            
            // Add more colors as needed
        };
        public static MemoryStream ExportAnsibe(Models.Ansibe ansibeToExport)
        {
            if (ansibeToExport == null) throw new ArgumentNullException("Ansibe is null");

            foreach (var group in ansibeToExport.Sections.GroupBy(s => s.Course))
            {
                myMap[group.Key.CourseCode]=new List<int>{ 1,group.Count()};
            }

            




            using (var workbook = new XLWorkbook())
            {

                var worksheet2 = workbook.Worksheets.Add("Sheet2");
                worksheet2.RightToLeft = true;
                int current_row2 = PrepareHeaderProfessorInCourse(worksheet2, ansibeToExport.Year);
                int startrow2 = current_row2;
                var sectionGroupedUnderCourse = ansibeToExport.Sections.GroupBy(s => s.Course);
                foreach (var group in sectionGroupedUnderCourse)
                {
                    try
                    {
                        current_row2 += addGroupCourse(worksheet2, group, current_row2);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }

                }
                worksheet2.SheetView.FreezeRows(7);
                var dataRange2 = worksheet2.Range(startrow2, 1, worksheet2.RangeUsed().RowCount(), worksheet2.RangeUsed().ColumnCount());
                dataRange2.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                dataRange2.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;



                var worksheet = workbook.Worksheets.Add("Sheet1");
                worksheet.RightToLeft = true;
                int current_row=PrepareHeaderCoursesInProfessor(worksheet,ansibeToExport.Year);
                int startrow = current_row;
                var sectionGroupedUnderProfessor = ansibeToExport.Sections.GroupBy(s => s.Professor);
                foreach (var group in sectionGroupedUnderProfessor)
                {
                    try { 
                      current_row+=  addGroupProfessors(worksheet, group, current_row); 
                    }
                    catch(Exception ex) 
                    {
                        throw new Exception(ex.Message);
                    }
                    
                }
                worksheet.SheetView.FreezeRows(5);
                var dataRange = worksheet.Range(startrow, 1, worksheet.RangeUsed().RowCount(), worksheet.RangeUsed().ColumnCount());
                dataRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                dataRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;






                // worksheet.Columns().AdjustToContents();
                //worksheet.Column(2).Width = 18;
                //worksheet.Column(17).Width = 15;
                // Save the Excel file
                //string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                //workbook.SaveAs(path);
                string fileName = "Ansibe"+ansibeToExport.Year + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

                // Save the workbook to a memory stream
                MemoryStream stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0; // Reset the stream position for reading

                // Prepare the response to send the Excel file as a download
               
                myMap.Clear();
                labeledSections.Clear();
                return stream;
            }


        }
        


        

        private static int addGroupCourse(IXLWorksheet worksheet, IGrouping<Course, Section> group, int current_row)
        {
            int work_row = current_row;
            int count = group.Count();
            int final_row = current_row + count;

           
            //عدد الساعات الشاغرة
            var merge_range1 = worksheet.Range(current_row, 1, final_row - 1, 1);
            merge_range1.Merge();
            worksheet.Cell(current_row, 1).Value = 0;



            //عدد الساعات المؤمنة
            var merge_range2 = worksheet.Range(current_row, 2, final_row - 1, 2);
            merge_range2.Merge();
            worksheet.Cell(current_row, 2).FormulaA1 = "=SUM(C" + current_row + ":C" + (final_row - 1) + ")";

            //مادة التدريس او المقرر
            var merge_range3 = worksheet.Range(current_row, 14, final_row - 1, 14);
            merge_range3.Merge();
            worksheet.Cell(current_row, 14).Value = group.Key.CourseDescription;

            //الفصل
            var merge_range4 = worksheet.Range(current_row, 15, final_row - 1, 15);
            merge_range4.Merge();
            worksheet.Cell(current_row, 15).Value = group.Key.Semester;

            //رمز المادة
            var merge_range5 = worksheet.Range(current_row, 17, final_row - 1, 17);
            merge_range5.Merge();
            worksheet.Cell(current_row, 17).Value = group.Key.CourseCode;

            

            foreach (Section section in group)
            {
                worksheet.Cell(work_row, 3).Value =section.Course.TotalNumberOfHours;
                Professor prof = section.Professor;
                if(prof!=null)
                {
                    // int ContractField = contractMapSheet2[prof.Contract.ContractType];
                    try
                    {
                        worksheet.Cell(work_row, contractMapSheet2[prof.Contract.ContractType]).Value = "X";
                    }catch(Exception e)
                    {
                        worksheet.Cell(work_row, 4).Value = "contract is unvaled";
                    }
                }
               // worksheet.Cell(work_row, 4).Value =(prof!=null)? prof.Contract.ContractType:"null prof";
                worksheet.Cell(work_row, 7).Value = (prof != null) ? prof.FullNameInArabic : "null prof";
                worksheet.Cell(work_row, 8).Value = 0;
                string sectionLabel= myMap[section.Course.CourseCode][0]++ + "\\" + myMap[section.Course.CourseCode][1];
                worksheet.Cell(work_row, 9).Value = sectionLabel;//work in map
                labeledSections[section.SectionId] = sectionLabel;
                worksheet.Cell(work_row, 10).Value = section.Language.ToString() ;
                worksheet.Cell(work_row, 11).Value = (section.TP!=null)? section.TP:"";
                worksheet.Cell(work_row, 12).Value = (section.TD != null) ? section.TD : "";
                worksheet.Cell(work_row, 13).Value = (section.CourseHours != null) ? section.CourseHours : "";
                worksheet.Cell(work_row, 16).Value = "-";
                work_row++;

            }


            var boldLineRange = worksheet.Range(final_row , 1, final_row , 17);
            boldLineRange.Style.Border.TopBorder = XLBorderStyleValues.Thick;
            boldLineRange.Style.Border.TopBorderColor = XLColor.Black;

            
            return work_row - current_row;
        }

        private static int PrepareHeaderProfessorInCourse(IXLWorksheet worksheet,string year)
        {
            // Set the custom headers
            worksheet.Cell(1, 1).Value = "الجامعة اللبنانية";
            worksheet.Cell(2, 1).Value = "كلية العلوم";
            worksheet.Cell(3, 1).Value = "الفرع الأول";
            worksheet.Cell(3, 7).Value = "  جدول توزيع مواد التدريس للعام الجامعي   "+ year;
            worksheet.Cell(4, 1).Value = "قسم الرياضيات التطبيقية";

            // Merge cells for the custom header
            var mergedRange = worksheet.Range("G3:P3");
            mergedRange.Merge();

            // Set the text alignment for merged cells
            mergedRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            mergedRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            for (int i = 1; i < 18; i++)
            {
                for (int j = 1; j < 5; j++)
                {
                    worksheet.Cell(j, i).Style.Font.Bold = true;
                    worksheet.Cell(j, i).Style.Fill.BackgroundColor = XLColor.LightGray;
                }
            }
            // Headers for the table
            var mergedRange1 = worksheet.Range("A5:A7");
            mergedRange1.Merge();
            worksheet.Cell(5, 1).Value = "عدد الساعات الشاغرة";


            var mergedRange2 = worksheet.Range("B5:B7");
            mergedRange2.Merge();
            worksheet.Cell(5, 2).Value = "عدد الساعات المؤمنة";




            var mergedRange3 = worksheet.Range("C5:G5");
            mergedRange3.Merge();
            worksheet.Cell(5, 3).Value = "الاستاذ";
            var mergedRange3_1 = worksheet.Range("C6:C7");
            mergedRange3_1.Merge();
            worksheet.Cell(6, 3).Value = "عدد الساعات";
            var mergedRange3_2 = worksheet.Range("D6:D7");
            mergedRange3_2.Merge();
            worksheet.Cell(6, 4).Value = "ساعة";
            var mergedRange3_3 = worksheet.Range("E6:E7");
            mergedRange3_3.Merge();
            worksheet.Cell(6, 5).Value = "تفرغ";
            var mergedRange3_4 = worksheet.Range("F6:F7");
            mergedRange3_4.Merge();
            worksheet.Cell(6, 6).Value = "ملاك";
            var mergedRange3_5 = worksheet.Range("G6:G7");
            mergedRange3_5.Merge();
            worksheet.Cell(6, 7).Value = "اسم الاستاذ";





            var mergedRange4 = worksheet.Range("H5:I5");
            mergedRange4.Merge();
            worksheet.Cell(5, 8).Value = "الشعب";
            var mergedRange4_1 = worksheet.Range("H6:H7");
            mergedRange4_1.Merge();
            worksheet.Cell(6, 8).Value = "عدد طلابها";
            var mergedRange4_2 = worksheet.Range("I6:I7");
            mergedRange4_2.Merge();
            worksheet.Cell(6, 9).Value = "الشعبة على عدد الشعب";




            var mergedRange5 = worksheet.Range("J5:J7");
            mergedRange5.Merge();
            worksheet.Cell(5, 10).Value = "لغة التدريس";



            var mergedRange6 = worksheet.Range("K5:M5");
            mergedRange6.Merge();
            worksheet.Cell(5, 11).Value = "عدد ساعات التدريس وفقاً للمنهج";
            var mergedRange6_1 = worksheet.Range("K6:K7");
            mergedRange6_1.Merge();
            worksheet.Cell(6, 11).Value = "اعمال تطبيقية";
            var mergedRange6_2 = worksheet.Range("L6:L7");
            mergedRange6_2.Merge();
            worksheet.Cell(6, 12).Value = "تدريب اعمال موجهة";
            var mergedRange6_3 = worksheet.Range("M6:M7");
            mergedRange6_3.Merge();
            worksheet.Cell(6, 13).Value = "نظري";



            var mergedRange7 = worksheet.Range("N5:N7");
            mergedRange7.Merge();
            worksheet.Cell(5, 14).Value = "مادة التدريس او المقرر";



            var mergedRange8 = worksheet.Range("O5:O7");
            mergedRange8.Merge();
            worksheet.Cell(5, 15).Value = "الفصل";



            var mergedRange9 = worksheet.Range("P5:P7");
            mergedRange9.Merge();
            worksheet.Cell(5, 16).Value = "الاختصاص";




            var mergedRange10 = worksheet.Range("Q5:Q7");
            mergedRange10.Merge();
            worksheet.Cell(5, 17).Value = "رمز المادة او المقرر";


            
            // Adjust column width if needed
            worksheet.Columns().AdjustToContents();
            worksheet.Column(2).Width = 18;
            worksheet.Column(3).Width = 10;
            worksheet.Column(4).Width = 10;
            worksheet.Column(5).Width = 10;
            worksheet.Column(6).Width = 10;
            worksheet.Column(7).Width = 10;
            worksheet.Column(8).Width = 10;
            worksheet.Column(9).Width = 18;
            worksheet.Column(10).Width = 15;
            worksheet.Column(11).Width = 10;
            worksheet.Column(12).Width = 12;
            worksheet.Column(13).Width = 10;
            worksheet.Column(14).Width = 40;
            worksheet.Column(15).Width = 10;
            worksheet.Column(16).Width = 10;
            worksheet.Column(17).Width = 15;
            var headerGroup = worksheet.Range("A5:Q7");
            headerGroup.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerGroup.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            headerGroup.Style.Font.Bold = true;

            return 8;
        }

        private static int addGroupProfessors(IXLWorksheet worksheet, IGrouping<Professor?, Section> group,int row)
        {
            int work_row = row;
            int count = group.Count();
            int final_row = row + count;
            Professor prof=group.Key;
            var merge_range = worksheet.Range(row,1,row+count-1,1);
            merge_range.Merge();
            worksheet.Cell(row, 1).Value = prof.FullNameInArabic;
           
            // add the group sections
            foreach (var section in group)
            {
                
                worksheet.Cell(work_row, 2).Value = prof.Rank.ToString();

                /*worksheet.Cell(work_row, 3).Value = prof.Contract.ContractType;
                worksheet.Cell(work_row, 4).Value = "-";
                worksheet.Cell(work_row, 5).Value = "-";*/
               
                    // int ContractField = contractMapSheet2[prof.Contract.ContractType];
                    try
                    {
                        worksheet.Cell(work_row, contractMapSheet1[prof.Contract.ContractType]).Value = "X";
                    }
                    catch (Exception e)
                    {
                        worksheet.Cell(work_row, 3).Value = "contract is unvaled";
                    }
                
                worksheet.Cell(work_row, 6).Value = prof.Speciality.ToString();
                worksheet.Cell(work_row, 7).Value = section.Course.CourseDescription;
                worksheet.Cell(work_row, 8).Value = section.Course.Semester;
                worksheet.Cell(work_row, 9).Value = section.Course.CourseCode;
                string fields = "";
                int total_hours=0;
                if (section.TD != null)
                {
                    fields += " TD ";
                    
                    total_hours += section.TD.Value;
                }
                if (section.TP != null)
                {
                    fields += (fields != "") ? "+" : "";
                    fields += " TP ";
                    
                    total_hours += section.TP.Value;
                }
                if (section.CourseHours != null)
                {
                    fields += (fields != "") ? "+" : "";
                    fields += " C ";
                    total_hours += section.CourseHours.Value;
                }
                worksheet.Cell(work_row, 10).Value = fields;
                worksheet.Cell(work_row, 11).Value = total_hours;
                worksheet.Cell(work_row, 12).Value = (section.Course.Semester / 2+ section.Course.Semester%2) ;
                worksheet.Cell(work_row, 13).Value = section.Language.ToString();
                worksheet.Cell(work_row, 14).Value = 0;
                worksheet.Cell(work_row, 15).Value = 0;
                worksheet.Cell(work_row, 16).Value = labeledSections[section.SectionId];

                worksheet.Cell(work_row, 17).Value = 1;
                worksheet.Cell(work_row, 18).Value = "العلوم 1";

                work_row++;
            }
            // try to add a fuction on last cell
            worksheet.Cell(row, 19).FormulaA1 = "=SUM(K" + row + ":K" + (work_row-1)+")";

            /*var range = worksheet.Range(row , 1, work_row , 19); // Adjust columns as per your data
            range.Style.Fill.BackgroundColor = groupColors[Color];
            Color = (Color + 1) % groupColors.Length;*/


            var boldLineRange = worksheet.Range(final_row, 1, final_row, 19);
            boldLineRange.Style.Border.TopBorder = XLBorderStyleValues.Thick;
            boldLineRange.Style.Border.TopBorderColor = XLColor.Black;

            var merge_range2 = worksheet.Range(row, 19, row + count - 1, 19);
            merge_range2.Merge();
            return work_row - row;
        }

        private static int PrepareHeaderCoursesInProfessor(IXLWorksheet worksheet,string year)
        {
            // Set the custom headers
            worksheet.Cell(1, 1).Value = "الجامعة اللبنانية";
            worksheet.Cell(2, 1).Value = "كلية العلوم";
            worksheet.Cell(3, 1).Value = "الفرع الأول";
            worksheet.Cell(3, 7).Value = "جدول تحديد نصاب الأستاذ للعام الجامعي "+year;
            worksheet.Cell(4, 1).Value = "قسم الرياضيات التطبيقية";

            // Merge cells for the custom header
            var mergedRange = worksheet.Range("G3:P3");
            mergedRange.Merge();

            // Set the text alignment for merged cells
            mergedRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            mergedRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            for (int i = 1; i < 20; i++)
            {
                for (int j = 1; j < 5; j++)
                {
                    worksheet.Cell(j, i).Style.Font.Bold = true;
                    worksheet.Cell(j, i).Style.Fill.BackgroundColor = XLColor.LightGray;
                }
            }
            // Headers for the table
            worksheet.Cell(5, 1).Value = "اسم الأستاذ";
            worksheet.Cell(5, 2).Value = "الرتبة";
            worksheet.Cell(5, 3).Value = "ملاك";
            worksheet.Cell(5, 4).Value = "تفرغ";
            worksheet.Cell(5, 5).Value = "ساعة";
            worksheet.Cell(5, 6).Value = "الاختصاص";
            worksheet.Cell(5, 7).Value = "المواد المسندة";
            worksheet.Cell(5, 8).Value = "الفصل";
            worksheet.Cell(5, 9).Value = "الرمز";
            worksheet.Cell(5, 10).Value = "طبيعية المادة *(3)";
            worksheet.Cell(5, 11).Value = "عدد الساعات";
            var mergedRange2 = worksheet.Range("L5:M5");
            mergedRange2.Merge();
            worksheet.Cell(5, 12).Value = "السنة المنهجية مع ذكر اللغة";
            worksheet.Cell(5, 14).Value = "عدد الطلاب في السنة المنهجية";
            worksheet.Cell(5, 15).Value = "عدد الطلاب الفعلي في الشعبة";
            worksheet.Cell(5, 16).Value = " عدد الشعب * (2) رقم الشعبة";

            worksheet.Cell(5, 17).Value = "(ترابط الاختصاص مع المواد المسندة * (2";
            worksheet.Cell(5, 18).Value = "الكلية والفرع";
            worksheet.Cell(5, 19).Value = "مجموع نصاب الأستاذ";
           
                // Adjust column width if needed
            worksheet.Columns().AdjustToContents();
            worksheet.Column(1).Width = 28;
            worksheet.Column(2).Width = 18;
            worksheet.Column(3).Width = 12;
            worksheet.Column(4).Width = 5;
            worksheet.Column(5).Width = 5;
            worksheet.Column(6).Width = 12;
            worksheet.Column(7).Width = 40;
            worksheet.Column(8).Width = 4;
            worksheet.Column(9).Width = 7;
            worksheet.Column(10).Width = 14;
            worksheet.Column(11).Width = 9;
            worksheet.Column(12).Width = 8;
            worksheet.Column(13).Width = 12;
            worksheet.Column(14).Width = 21;
            worksheet.Column(15).Width = 20;
            worksheet.Column(16).Width = 21;
            worksheet.Column(17).Width = 30;
            worksheet.Column(18).Width = 8;
            worksheet.Column(19).Width = 15;
            var headerGroup = worksheet.Range("A5:S5");
            headerGroup.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerGroup.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            headerGroup.Style.Font.Bold = true;
            return 6;
        }

    }

       
    }



