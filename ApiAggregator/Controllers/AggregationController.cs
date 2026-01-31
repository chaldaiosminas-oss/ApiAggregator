using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiAggregator.Controllers
{
    [ApiController]
    [Route("api/aggregate")]
    public class AggregationController : ControllerBase
    {
        private readonly AggregationService _service;

        public AggregationController(AggregationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? sort, [FromQuery] string? category)
        {
            var data = await _service.AggregateAsync(sort, category);
            return Ok(data);
        }
    }

}
