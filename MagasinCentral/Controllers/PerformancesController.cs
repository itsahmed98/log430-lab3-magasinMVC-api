using MagasinCentral.Services;
using Microsoft.AspNetCore.Mvc;

namespace MagasinCentral.Controllers
{
    /// <summary>
    /// Contrôleur pour les performances du tableau de bord.
    /// </summary>
    public class PerformancesController : Controller
    {
        private readonly IPerformancesService _performancesService;
        private readonly ILogger<PerformancesController> _logger;

        public PerformancesController(IPerformancesService performancesService, ILogger<PerformancesController> logger)
        {
            _performancesService = performancesService ?? throw new ArgumentNullException(nameof(performancesService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Affiche le tableau de bord des performances.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            IActionResult? result = null;

            _logger.LogInformation("Tentative de récupération des performances...");

            try
            {
                var model = await _performancesService.GetPerformances();
                _logger.LogInformation("Performaces ont été récupérées avec succès.");
                result = View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur s'est produite lors de la récupération des performances.");
                result = View("Error");
            }

            return result;
        }
    }
}