using ApiAggregator.Models;
using ApiAggregator.Services;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

public class WeatherService : IExternalApiService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly StatisticsService _stats;
    private readonly IConfiguration _config;

    public string Name => "OpenWeather";

    public WeatherService(
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
        return await _cache.GetOrCreateAsync("weather_data", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            var sw = Stopwatch.StartNew();

            try
            {
                var key = _config["ApiKeys:WeatherApi"];
                var url = $"https://api.openweathermap.org/data/2.5/weather?q=London&appid={key}&units=metric";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                using var doc = System.Text.Json.JsonDocument.Parse(json);

                sw.Stop();
                _stats.Record(Name, sw.ElapsedMilliseconds);

                return new[]
                {
                    new UnifiedResult
                    {
                        Source = Name,
                        Title = "Weather in London",
                        Description = $"Temperature: {doc.RootElement.GetProperty("main").GetProperty("temp").GetDecimal()}°C",
                        Date = DateTime.UtcNow,
                        Category = "weather"
                    }
                };
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
