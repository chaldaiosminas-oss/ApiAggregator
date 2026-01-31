using ApiAggregator.Models;
using ApiAggregator.Services;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

public class NewsService : IExternalApiService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly StatisticsService _stats;
    private readonly IConfiguration _config;

    public string Name => "NewsAPI";

    public NewsService(
        IHttpClientFactory factory,
        IMemoryCache cache,
        StatisticsService stats,
        IConfiguration config)
    {
        _httpClient = factory.CreateClient("external");
        _cache = cache;
        _stats = stats;
        _config = config;
    }

    public async Task<IEnumerable<UnifiedResult>> FetchAsync()
    {
        return await _cache.GetOrCreateAsync("news_data", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            var sw = Stopwatch.StartNew();

            try
            {
                var apiKey = _config["ApiKeys:NewsApi"];
                var url = $"https://newsapi.org/v2/everything?q=technology&apiKey={apiKey}";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                using var doc = System.Text.Json.JsonDocument.Parse(json);

                sw.Stop();
                _stats.Record(Name, sw.ElapsedMilliseconds);

                return doc.RootElement.GetProperty("articles")
                    .EnumerateArray()
                    .Take(5)
                    .Select(a => new UnifiedResult
                    {
                        Source = Name,
                        Title = a.GetProperty("title").GetString()!,
                        Description = a.GetProperty("description").GetString() ?? "",
                        Date = a.GetProperty("publishedAt").GetDateTime(),
                        Category = "news"
                    })
                    .ToList();
            }
            catch
            {
                sw.Stop();
                _stats.Record(Name, sw.ElapsedMilliseconds);
                return Enumerable.Empty<UnifiedResult>();
            }
        })!;
    }
}
