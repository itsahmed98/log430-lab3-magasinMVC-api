using Microsoft.AspNetCore.Mvc;
using RapportMcService.Models;
using RapportMcService.Services;

namespace RapportMcService.Controllers
{
    [ApiController]
    [Route("api/v1/rapports")]
    public class RapportController : ControllerBase
    {
        private readonly IRapportService _rapportService;
        private readonly ILogger<RapportController> _logger;

        public RapportController(ILogger<RapportController> logger, IRapportService rapportService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _rapportService = rapportService ?? throw new ArgumentNullException(nameof(rapportService));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RapportDto>>> GetRapport()
        {
            _logger.LogInformation("Début de la récupération du rapport consolidé...");
            var rapports = await _rapportService.ObtenirRapportConsolideAsync();
            if (rapports == null || !rapports.Any())
            {
                _logger.LogWarning("Aucun rapport trouvé.");
                return NotFound("Aucun rapport disponible.");
            }
            _logger.LogInformation("Rapport consolidé récupéré avec succès");
            return Ok(rapports);
        }
    }
}