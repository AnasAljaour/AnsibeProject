using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnsibeProject.Models
{
    public enum Rank
    {

        [Display(Name ="أستاذ مساعد")]
        [Description("أستاذ مساعد")]
        AssistantProfessor,


        [Display(Name ="أستاذ معيد")]
        [Description("أستاذ معيد")]
        AssociateProfessor,

        [Display(Name ="أستاذ")]
        [Description("أستاذ")]
        Professor
    }
}
