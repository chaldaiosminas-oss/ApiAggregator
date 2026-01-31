using ApiAggregator.Models;
using ApiAggregator.Services;

public class AggregationService
{
    private readonly IEnumerable<IExternalApiService> _services;

    public AggregationService(IEnumerable<IExternalApiService> services)
    {
        _services = services;
    }

    public async Task<IEnumerable<UnifiedResult>> AggregateAsync(
        string? sort,
        string? category)
    {
        var tasks = _services.Select(s => s.FetchAsync());
        var results = await Task.WhenAll(tasks);

        var aggregated = results.SelectMany(r => r);

        if (!string.IsNullOrEmpty(category))
            aggregated = aggregated
                .Where(r => r.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

        aggregated = sort switch
        {
            "date" => aggregated.OrderByDescending(r => r.Date),
            _ => aggregated
        };

        return aggregated;
    }
}
