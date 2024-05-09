using AnsibeProject.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace AnsibeProject.Models
{
    public class Course
    {
        [Required(ErrorMessage ="Course Code is required /:")]
        [Key]
        [MaxLength(50)]
        public string CourseCode { get; set; } = string.Empty;



        // public List<Course> PreRequistedCourses { get; set; }=new List<Course>();


        [Required(ErrorMessage = "Course Discription is required" )]
        [MaxLength(255, ErrorMessage = "Course Discription must be at most 1 line, 255")]
        public string CourseDescription { get; set; } = string.Empty;
       
        
        [Required(ErrorMessage = "Course Credit is required ")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "number of hours must be a positive integer !")]
        public  int NumberOfCredits { get; set; }
        [Required(ErrorMessage = "Course Total hours is required ")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "number of hours must be a positive integer !")]
        [TotalNumberOfHoursNotExceed("TotalNumberOfHours")]
        public  int TotalNumberOfHours { get; set; }


        [Required(ErrorMessage = "Course hours is required ")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage= "number of hours must be a positive integer !")]
        public  int NumberOfHours { get; set; }

        
        [Required(ErrorMessage = "Course TP hours is required ")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "number of hours must be a positive integer !")]
        public int TP { get; set; }


        [Required(ErrorMessage = "Course TD hours is required ")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "number of hours must be a positive integer !")]

        public int TD { get; set; }

        [Required(ErrorMessage = "Course semester is required ")]
        [RegularExpression(@"^[1-8]$",ErrorMessage="semester out of range 1-8 !")]
        public int Semester {  get; set; }

        [Required(ErrorMessage = "Course state is required ")]
        public ActiveState CourseState { get; set; } = ActiveState.Active;


        [Required(ErrorMessage = "Course Major is required ")]
        public CourseMajor Major { get; set; }

        [Required(ErrorMessage = "Course Obligatory is required ")]
        public CourseObligatory Obligatory { get; set; } = CourseObligatory.Mandatory;


    }
    
    class TotalNumberOfHoursNotExceed : ValidationAttribute
    {
        private readonly string _totalHoursPropertyName;
        public TotalNumberOfHoursNotExceed(string totalHoursPropertyName)
        {
            _totalHoursPropertyName = totalHoursPropertyName;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var propertyInfo = validationContext.ObjectType.GetProperty(_totalHoursPropertyName);
            var tdProperty = validationContext.ObjectType.GetProperty("TD");
            var tpProperty = validationContext.ObjectType.GetProperty("TP");
            var courseProperty = validationContext.ObjectType.GetProperty("NumberOfHours");
            if (propertyInfo == null)
            {
                return new ValidationResult($"Unkown property : {_totalHoursPropertyName}");
            }
            var totalHoureValue = (int)propertyInfo.GetValue(validationContext.ObjectInstance);
            var tdValue = (int)tdProperty.GetValue(validationContext.ObjectInstance);
            var tpValue = (int)tpProperty.GetValue(validationContext.ObjectInstance);
            var courseValue = (int)courseProperty.GetValue(validationContext.ObjectInstance);
            if ((tdValue + tpValue + courseValue) == totalHoureValue)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult(ErrorMessage ?? $"The sum of course, TD and TB must be  equal to {totalHoureValue}.");
        }
    }


}

