using System.ComponentModel.DataAnnotations;

namespace AnsibeProject.Models
{
    public enum Rank
    {

        [Display(Name ="Assistant Professor")]
        AssistantProfessor,


        [Display(Name ="Associate Professore")]
        AssociateProfessore,

        Professor,
    }
}
