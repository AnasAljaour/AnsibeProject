namespace AnsibeProject.Models
{
    public class AssignToggleRequest
    {
        public KeyValuePairModel AnsibeId { get; set; }
        public List<KeyValuePairModel> Allocation { get; set; }
        public string Type { get; set; }
    }
}
