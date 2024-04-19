using AnsibeProject.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace AnsibeProject.Models
{
    public class Course
    {
        [Required]
        public string CourseCode { get; set; }



        // public List<Course> PreRequistedCourses { get; set; }=new List<Course>();


        [Required]
        public  string CourseDescription { get; set; }
       
        
        [Required]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "number of hours must be a positive integer !")]
        public required int NumberOfCredits { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "number of hours must be a positive integer !")]
        public required int TotalNumberOfHours { get; set; }


        [Required]
        [RegularExpression(@"^[0-9]+$", ErrorMessage= "number of hours must be a positive integer !")]
        [TotalNumberOfHoursNotExceed("TotalNumberOfHours")]
        public required int NumberOfHours { get; set; }

        
        [Required]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "number of hours must be a positive integer !")]
        [TotalNumberOfHoursNotExceed("TotalNumberOfHours")]
        public int TP { get; set; }


        [Required]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "number of hours must be a positive integer !")]
        [TotalNumberOfHoursNotExceed("TotalNumberOfHours")]
        public int TD { get; set; }

        [Required]
        [RegularExpression(@"^[1-8]$",ErrorMessage="semester out of range 1-8 !")]
        public int Semester {  get; set; }

        [Required]
        public ActiveState CourseState { get; set; } = ActiveState.Active;


        [Required]
        public CourseMajor Major { get; set; }

        [Required]
        public CourseObligatory Obligatory { get; set; } = CourseObligatory.Mandatory;


    }

    class TotalNumberOfHoursNotExceed: ValidationAttribute
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
            var tpProperty = validationContext.ObjectType.GetProperty("TB");
            var courseProperty = validationContext.ObjectType.GetProperty("NumberOfHours");
            if (propertyInfo == null)
            {
                return new ValidationResult($"Unkown property : {_totalHoursPropertyName}");
            }
            var totalHoureValue = (int) propertyInfo.GetValue(validationContext.ObjectInstance);
            var tdValue = (int)tdProperty.GetValue(validationContext.ObjectInstance);
            var tpValue = (int) tpProperty.GetValue(validationContext.ObjectInstance);
            var courseValue = (int)courseProperty.GetValue(validationContext.ObjectInstance);
            if ((tdValue+tpValue+courseValue)<=totalHoureValue)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult(ErrorMessage ?? $"The sum of course, TD and TB must be less than or equal to {totalHoureValue}.");
        }
    }

    
}

