namespace ApiAggregator.Models
{
    public class ApiStats
    {
        public int TotalRequests { get; set; }
        public List<long> ResponseTimes { get; set; } = new();
    }
}
