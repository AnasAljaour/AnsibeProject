using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnsibeProject.Models
{
    public class Ansibe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 
        [MaxLength(10)]
        public string Year { get; set; } = string.Empty;
        public ICollection<Section> Sections { get; set; } = new List<Section>();
    }
}
