using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiAggregator.Controllers
{
    [ApiController]
    [Route("api/stats")]
    public class StatsController : ControllerBase
    {
        private readonly StatisticsService _stats;

        public StatsController(StatisticsService stats)
        {
            _stats = stats;
        }

        [HttpGet]
        public IActionResult Get() => Ok(_stats.GetStats());
    }

}
