using Microsoft.AspNetCore.Mvc;
using PerformancesMcService.Models;
using PerformancesMcService.Services;

namespace PerformancesMcService.Controllers
{
    [ApiController]
    [Route("api/v1/performances")]
    public class PerformanceController : ControllerBase
    {
        private readonly IPerformanceService _performanceService;
        private readonly ILogger<PerformanceController> _logger;

        public PerformanceController(IPerformanceService performanceService, ILogger<PerformanceController> logger)
        {
            _performanceService = performanceService;
            _logger = logger;
        }

        /// <summary>
        /// Retourner toutes les performances.
        /// </summary>
        // GET: api/v1/performances
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var list = await _performanceService.GetAllPerformancesAsync();
            return Ok(list);
        }
    }
}
