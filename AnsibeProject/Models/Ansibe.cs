using System.ComponentModel.DataAnnotations;

namespace AnsibeProject.Models
{
    public class Ansibe
    {
        [Key]
        [MaxLength(25)]
        public string Id { get; set; } = string.Empty;
        [MaxLength(10)]
        public string Year { get; set; } = string.Empty;
        public ICollection<Section> Sections { get; set; } = new List<Section>();
    }
}
