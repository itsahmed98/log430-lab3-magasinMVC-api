using Microsoft.AspNetCore.Mvc;
using MagasinCentral.Services;
using MagasinCentral.Models;

namespace MagasinCentral.Api.Controllers
{
    /// <summary>
    /// Contrôleur pour la gestion des produits.
    /// </summary>
    [ApiController]
    [Route("api/v1/produits")]
    public class ProduitController : ControllerBase
    {
        private readonly IProduitService _produitService;
        private readonly ILogger<ProduitController> _logger;

        public ProduitController(ILogger<ProduitController> logger, IProduitService produitService)
        {
            _produitService = produitService ?? throw new ArgumentNullException(nameof(produitService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        }

        /// <summary>
        /// Récupérer la liste de tous les produits.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Produits()
        {
            return Ok(await _produitService.GetAllProduitsAsync());
        }


        /// <summary>
        /// Récupérer un produit par son ID.
        /// Si le produit n'existe pas, retourne 404 Not Found.
        /// </summary>
        /// <param name="produitId"></param>
        /// <returns></returns>
        [HttpGet("{produitId:int}")]
        public async Task<IActionResult> Produit(int produitId)
        {
            var produit = await _produitService.GetProduitByIdAsync(produitId);
            if (produit == null)
            {
                return NotFound();
            }
            return Ok(produit);
        }

        /// <summary>
        /// Mettre à jour un produit existant.
        /// </summary>
        /// <param name="produitId">ID du produit à modifier.</param>
        /// <param name="payload">Les nouvelles données du produit.</param>
        /// 
        /// test avec:
        /// {
        ///    "produitId": 3,
        ///    "nom": "Clé USB 32 Go",
        ///    "categorie": "Électronique",
        ///    "prix": 15.00,
        ///    "description": "Clé USB 32 Go avec protection améliorée333"
        /// }
        [HttpPut("{produitId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Modifier(
            [FromRoute] int produitId,
            [FromBody] Produit payload)
        {
            Console.WriteLine($"Modifier produit ID={produitId} avec données: {payload}");

            if (produitId != payload.ProduitId)
                return BadRequest($"L’ID de l’URL ({produitId}) doit correspondre à celui du corps de la requête ({payload.ProduitId}).");

            try
            {
                await _produitService.ModifierProduitAsync(payload);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Produit introuvable pour ID={ProduitId}", produitId);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur inconnue lors de la modification du produit ID={ProduitId}", produitId);
                return StatusCode(500, "Une erreur s'est produite côté serveur.");
            }
        }
    }
}

