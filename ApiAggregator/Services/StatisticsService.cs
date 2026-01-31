using ApiAggregator.Models;
using System.Collections.Concurrent;

public class StatisticsService
{
    private readonly ConcurrentDictionary<string, ApiStats> _stats = new();

    public void Record(string apiName, long responseTime)
    {
        var apiStats = _stats.GetOrAdd(apiName, _ => new ApiStats());
        lock (apiStats)
        {
            apiStats.TotalRequests++;
            apiStats.ResponseTimes.Add(responseTime);
        }
    }

    public object GetStats()
    {
        return _stats.ToDictionary(
            s => s.Key,
            s => new
            {
                s.Value.TotalRequests,
                Fast = s.Value.ResponseTimes.Count(t => t < 100),
                Average = s.Value.ResponseTimes.Count(t => t >= 100 && t <= 200),
                Slow = s.Value.ResponseTimes.Count(t => t > 200)
            });
    }
}
