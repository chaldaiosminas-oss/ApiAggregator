namespace ApiAggregator.Models
{
    public class UnifiedResult
    {
        public string Source { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime? Date { get; set; }
        public string Category { get; set; } = default!;
    }

}
