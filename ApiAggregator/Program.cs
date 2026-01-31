using ApiAggregator.BackgroundServices;
using ApiAggregator.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // required for Swagger
builder.Services.AddSwaggerGen();           // registers ISwaggerProvider

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<StatisticsService>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IExternalApiService, GitHubService>();
builder.Services.AddScoped<IExternalApiService, NewsService>();
builder.Services.AddScoped<IExternalApiService, WeatherService>();
builder.Services.AddScoped<AggregationService>();
builder.Services.AddHostedService<PerformanceMonitorService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

