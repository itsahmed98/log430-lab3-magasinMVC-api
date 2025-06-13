using MagasinCentral.Models;
using MagasinCentral.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MagasinCentral.Controllers
{
    /// <summary>
    ///     Contrôleur pour UC6 : approbation/refus des demandes de réapprovisionnement.
    /// </summary>
    public class TraiterDemandesController : Controller
    {
        private readonly IReapprovisionnementService _reapprovisionnementService;
        private readonly ILogger<TraiterDemandesController> _logger;

        public TraiterDemandesController(ILogger<TraiterDemandesController> logger, IReapprovisionnementService reapprovisionnementService)
        {
            _reapprovisionnementService = reapprovisionnementService
                ?? throw new ArgumentNullException(nameof(reapprovisionnementService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Affiche la liste des demandes en attente.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            IActionResult? result = null;

            _logger.LogInformation("Récupération des demandes en attente...");

            try
            {
                var demandes = await _reapprovisionnementService.GetDemandesEnAttenteAsync();
                result = View(demandes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des demandes en attente.");
                TempData["Erreur"] = "Une erreur s'est produite lors de la récupération des demandes.";
                result = View(new List<DemandeReapprovisionnement>());
            }

            return result;
        }

        /// <summary>
        /// Traiter une demande en attente.
        /// </summary>
        /// <param name="demandeId">ID de la demande à traiter</param>
        /// <param name="approuver">true = approuver, false = refuser</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Traiter(int demandeId, bool approuver)
        {
            try
            {
                await _reapprovisionnementService.TraiterDemandeAsync(demandeId, approuver);
                TempData["Succès"] = approuver
                    ? "La demande a été approuvée avec succès."
                    : "La demande a été refusée.";
            }
            catch (Exception ex)
            {
                TempData["Erreur"] = $"Impossible de traiter la demande : {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
