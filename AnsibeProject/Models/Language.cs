using System.ComponentModel.DataAnnotations;

namespace AnsibeProject.Models
{
    public enum Language
    {
        [Display(Name = "E")]
        English,
        [Display(Name = "F")]
        French,
        [Display(Name = "E/F")]
        English_French
    }
}
