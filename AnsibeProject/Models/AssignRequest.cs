namespace AnsibeProject.Models
{
    public class AssignRequest
    {
        public KeyValuePairModel? AnsibeId { get; set; }
        public List<KeyValuePairModel>? Allocation { get; set; }
        public string? Type { get; set; }
    }
}
