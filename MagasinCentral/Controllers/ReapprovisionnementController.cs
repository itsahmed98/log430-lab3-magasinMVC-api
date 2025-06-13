using MagasinCentral.Services;
using Microsoft.AspNetCore.Mvc;

namespace MagasinCentral.Controllers
{
    /// <summary>
    /// Controller pour consulter le stock et demander un réapprovisionnement.
    /// </summary>
    public class ReapprovisionnementController : Controller
    {
        private readonly IReapprovisionnementService _reapprisonnementService;

        private readonly ILogger<ReapprovisionnementController> _logger;

        public ReapprovisionnementController(ILogger<ReapprovisionnementController> logger, IReapprovisionnementService reapprisonnementService)
        {
            _reapprisonnementService = reapprisonnementService ?? throw new ArgumentNullException(nameof(reapprisonnementService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Affiche la liste des produits avec stock local et central pour un magasin donné.
        /// </summary>
        /// <summary>
        ///     Affiche la liste des produits avec stock local et central pour un magasin donné.
        /// </summary>
        public async Task<IActionResult> Index(int magasinId)
        {
            var listeStocks = await _reapprisonnementService.GetStocksAsync(magasinId);

            ViewData["MagasinId"] = magasinId;
            return View(listeStocks);
        }

        /// <summary>
        /// Crée une demande de réapprovisionnement pour un produit donné.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DemanderReapprovisionnement(int magasinId, int produitId, int quantite)
        {
            _logger.LogInformation("Création d'une demande de réapprovisionnement : Magasin ID {MagasinId}, Produit ID {ProduitId}, Quantité {Quantite}.",
                magasinId, produitId, quantite);

            try
            {
                await _reapprisonnementService.CreerDemandeReapprovisionnementAsync(magasinId, produitId, quantite);
                _logger.LogInformation("Demande de réapprovisionnement créée avec succès pour le produit {ProduitId}.");

                TempData["Succès"] = $"Demande de réapprovisionnement créée pour le produit: {produitId} pour une quantité de : {quantite} unités.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de la demande de réapprovisionnement pour le produit ID {ProduitId}.");
                TempData["Erreur"] = $"Impossible de créer la demande : {ex.Message}";
            }

            return RedirectToAction(nameof(Index), new { magasinId });
        }

        /// <summary>
        /// Affiche la liste des demandes de réapprovisionnement.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> DemandesReapprovisionnement()
        {
            IActionResult? result = null;
            _logger.LogInformation("Affichage des demandes de réapprovisionnement.");

            try
            {
                var demandes = await _reapprisonnementService.GetDemandesReapprovisionnementAsync();
                result = View(demandes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des demandes de réapprovisionnement.");
                TempData["Erreur"] = "Erreur lors de la récupération des demandes de réapprovisionnement.";
                result = View(new List<object>());
            }

            return result;
        }
    }
}