using ApiAggregator.Models;
using ApiAggregator.Services;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using System.Net.Http.Headers;

public class GitHubService : IExternalApiService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly StatisticsService _stats;

    public string Name => "GitHub";

    public GitHubService(
        IHttpClientFactory factory,
        IMemoryCache cache,
        StatisticsService stats)
    {
        _httpClient = factory.CreateClient("external");
        _cache = cache;
        _stats = stats;

        _httpClient.DefaultRequestHeaders.UserAgent
            .Add(new ProductInfoHeaderValue("ApiAggregator", "1.0"));
    }

    public async Task<IEnumerable<UnifiedResult>> FetchAsync()
    {
        return await _cache.GetOrCreateAsync("github_data", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

            var stopwatch = Stopwatch.StartNew();
            try
            {
                var response = await _httpClient.GetAsync(
                    "https://api.github.com/search/repositories?q=dotnet&sort=stars");

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                using var document = System.Text.Json.JsonDocument.Parse(json);

                stopwatch.Stop();
                _stats.Record(Name, stopwatch.ElapsedMilliseconds);

                

                return document.RootElement
                    .GetProperty("items")
                    .EnumerateArray()
                    .Take(5)
                    .Select(repo => new UnifiedResult
                    {
                        Source = Name,
                        Title = repo.GetProperty("name").GetString()!,
                        Description = repo.TryGetProperty("description", out var desc)
                            ? desc.GetString() ?? ""
                            : "",
                        Date = repo.GetProperty("created_at").GetDateTime(),
                        Category = "technology"
                    })
                    .ToList();
            }
            catch
            {
                stopwatch.Stop();
                _stats.Record(Name, stopwatch.ElapsedMilliseconds);
                return Enumerable.Empty<UnifiedResult>();
            }
        })!;
    }
}
