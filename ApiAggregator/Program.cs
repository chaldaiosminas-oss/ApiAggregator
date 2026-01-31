using ApiAggregator.BackgroundServices;
using ApiAggregator.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<StatisticsService>();

builder.Services.AddScoped<IExternalApiService, GitHubService>();
builder.Services.AddScoped<IExternalApiService, NewsService>();
builder.Services.AddScoped<IExternalApiService, WeatherService>();

builder.Services.AddScoped<AggregationService>();
builder.Services.AddHostedService<PerformanceMonitorService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
