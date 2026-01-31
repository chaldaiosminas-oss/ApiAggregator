namespace ApiAggregator.BackgroundServices
{
    public class PerformanceMonitorService : BackgroundService
    {
        private readonly StatisticsService _stats;
        private readonly ILogger<PerformanceMonitorService> _logger;

        public PerformanceMonitorService(
            StatisticsService stats,
            ILogger<PerformanceMonitorService> logger)
        {
            _stats = stats;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var stats = _stats.GetStats();
                _logger.LogInformation("Performance snapshot: {@stats}", stats);
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }

}
