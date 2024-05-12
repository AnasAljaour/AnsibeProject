using System.ComponentModel.DataAnnotations;

namespace AnsibeProject.Models
{
    public class Section
    {
        [Key]
        [Display(Name = "Section ID")]
        [NonEmptySection]
        public int SectionId { get; set; }

        [Required]
        public Course Course { get; set; }

        public Professor? Professor { get; set; }

        public int? TP { get; set; }
        public int? TD { get; set; }
        public int? CourseHours { get; set; }

        public Language Language { get; set; }



    }
    class NonEmptySection : ValidationAttribute
    {
        public NonEmptySection() { }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var section = (Section)validationContext.ObjectInstance;

            // Check if any of TD, TP, or CourseHours are not null
            if (section.TD != null || section.TP != null || section.CourseHours != null)
            {
                return  ValidationResult.Success;
            }
            return new ValidationResult(ErrorMessage ?? $"At least one Type must be selected TP TD or Course");
        }
    }
}
