using Microsoft.AspNetCore.Mvc;
using MagasinCentral.Services;

namespace MagasinCentral.Controllers
{
    /// <summary>
    /// Contrôleur pour les rapports consolidés des ventes.
    /// </summary>
    public class RapportController : Controller
    {
        private readonly IRapportService _rapportService;
        private readonly ILogger<RapportController> _logger;

        /// <summary>
        /// Constructeur de <see cref="RapportController"/>.
        /// </summary>
        /// <param name="rapportService"></param>
        public RapportController(ILogger<RapportController> logger, IRapportService rapportService)

        {
            _rapportService = rapportService ?? throw new ArgumentNullException(nameof(rapportService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Affiche la page d'accueil du rapport consolidé.
        /// </summary>
        public IActionResult Index()
        {
            IActionResult? result = null!;

            try
            {
                _logger.LogInformation("Tentative de récupération du rapport consolidé...");
                var rapportConsolide = _rapportService.ObtenirRapportConsolideAsync().Result;

                if (rapportConsolide == null)
                {
                    _logger.LogWarning("Aucun rapport consolidé trouvé.");
                    result = View("NotFound");
                }
                else
                {
                    _logger.LogInformation("Rapport consolidé récupéré avec succès.");
                    result = View(rapportConsolide);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur s'est produite lors de la récupération du rapport consolidé.");
                ViewBag.ErrorMessage = $"Une erreur est survenue lors de la génération du rapport: {ex.Message}";
                result = View("Error");
            }

            return result;
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
