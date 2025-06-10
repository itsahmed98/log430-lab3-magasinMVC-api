using Microsoft.AspNetCore.Mvc;
using MagasinCentral.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MagasinCentral.Api.Controllers
{
    /// <summary>
    /// Contrôleur pour visualiser les performances des magasins.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/v1/performances")]
    public class PerformancesController : ControllerBase
    {
        private readonly IPerformancesService _performancesService;
        private readonly ILogger<PerformancesController> _logger;

        public PerformancesController(ILogger<PerformancesController> logger, IPerformancesService performancesService)
        {
            _performancesService = performancesService ?? throw new ArgumentNullException(nameof(performancesService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtenir les performances des magasins.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<IActionResult> GetPerformances()
        {
            IActionResult result = null!;

            try
            {
                result = Ok(await _performancesService.GetPerformances());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des performances.");
                result = StatusCode(500, "Une erreur s'est produite lors de la récupération des performances.");
            }

            return result;
        }
    }
}