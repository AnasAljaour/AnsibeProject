using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnsibeProject.Models
{
    public class Contract
    {
        [Required(ErrorMessage = "Contract Type is required")]
        [Key]
        [MaxLength(100)]
        [ForeignKey("Professor")]
        public string ContractType { get; set; } = string.Empty;

        [Display(Name = "Max Hours")]
        public int? MaxHours { get; set; }

        [Display(Name = "Min Hours")]
        public int? MinHours { get; set; }

        public ICollection<Professor> Professors { get; set; } = new List<Professor>();

        internal static Contract Parse(string v)
        {
            throw new NotImplementedException();
        }
    }
}
