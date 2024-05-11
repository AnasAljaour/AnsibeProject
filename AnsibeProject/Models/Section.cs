using System.ComponentModel.DataAnnotations;

namespace AnsibeProject.Models
{
    public class Section
    {
        [Key]
        [Display(Name = "Section ID")]
        public int SectionId { get; set; }

        [Required]
        public Course Course { get; set; }

        public Professor? Professor { get; set; }

        public int? TP { get; set; }
        public int? TD { get; set; }
        public int? CourseHours { get; set; }

        public Language Language { get; set; }



    }
}
