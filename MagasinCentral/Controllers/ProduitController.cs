using MagasinCentral.Services;
using MagasinCentral.ViewModels;
using Microsoft.AspNetCore.Mvc;
using MagasinCentral.Models;

namespace MagasinCentral.Controllers
{
    /// <summary>
    /// Contrôleur pour les performances du tableau de bord (UC3).
    /// </summary>
    public class ProduitController : Controller
    {
        private readonly ILogger<ProduitController> _logger;
        private readonly IProduitService _produitService;

        public ProduitController(ILogger<ProduitController> logger, IProduitService produitService)
        {
            _produitService = produitService ?? throw new ArgumentNullException(nameof(produitService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Afficher la liste des produits disponibles.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            IActionResult? result = null;
            _logger.LogInformation("Tentative de récupération des produits...");

            try
            {
                var model = await _produitService.GetAllProduitsAsync();
                result = View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur s'est produite lors de la récupération des produits.");
                result = View("Error");
            }

            return result;
        }

        /// <summary>
        /// Modifier un produit existant.
        /// </summary>
        /// <param name="produitId"></param>
        public async Task<IActionResult> Modifier(int produitId)
        {
            IActionResult? result = null;

            try
            {
                var produit = await _produitService.GetProduitByIdAsync(produitId);
                if (produit == null)
                {
                    result = NotFound($"Produit avec ID={produitId} non trouvé.");
                }
                result = View(produit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du produit pour modification.");
                result = View("Error");
            }

            return result;
        }

        /// <summary>
        /// Modifier un produit existant avec les données du formulaire.
        /// </summary>
        /// <param name="produit"></param>
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Modifier(Produit produit)
        {
            IActionResult? result = null;

            _logger.LogInformation("Début de la modification du produit avec l'ID {ProduitId}.", produit.ProduitId);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Le modèle pour le produit ID {ProduitId} est invalide.", produit.ProduitId);
                result = View(produit);
            }

            try
            {
                await _produitService.ModifierProduitAsync(produit);
                TempData["Succès"] = $"Le produit « {produit.Nom} » a bien été mis à jour.";
                _logger.LogInformation("Le produit ID {ProduitId} a été mis à jour avec succès.", produit.ProduitId);
                result = RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du produit ID {ProduitId}.", produit.ProduitId);
                result = View("Error");
            }

            return result;
        }

        /// <summary>
        /// Recherche de produits par identifiant, nom ou catégorie.
        /// </summary>
        /// <param name="produit"></param>
        /// <returns></returns>
        public async Task<IActionResult> Recherche(string produit)
        {
            IActionResult? result = null;

            _logger.LogInformation("Recherche initiée pour le produit : {produit}.", produit);

            if (string.IsNullOrWhiteSpace(produit))
            {
                _logger.LogWarning("Recherche annulée : le terme de recherche est vide ou nul.");
                result = View(new List<Produit>());
            }

            try
            {
                var résultats = await _produitService.RechercherProduitsAsync(produit);
                _logger.LogInformation("{Nombre} résultats trouvés pour le terme : {Terme}.", résultats.Count(), produit);

                ViewData["Terme"] = produit;
                result = View(résultats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la recherche du produit: {produit}", produit);
                return View(new List<Produit>());
            }

            return result;
        }
    }
}