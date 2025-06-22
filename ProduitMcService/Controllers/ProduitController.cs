using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProduitMcService.Models;
using ProduitMcService.Services;

namespace ProduitMcService.Controllers
{
    /// <summary>
    /// Fournit des endpoints pour gérer les produits.
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
        /// Retourner tous les produits.
        /// </summary>
        /// <returns></returns>
        // GET: api/v1/produits
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Récupération de tous les produits.");
            var list = await _produitService.GetAllAsync();
            _logger.LogInformation("{Count} produits récupérés.", list.Count);
            return Ok(list);
        }

        /// <summary>
        /// Retourner un produit par son ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/v1/produits/{id}
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Recherche du produit avec ID {Id}.", id);
            var p = await _produitService.GetByIdAsync(id);
            if (p == null)
            {
                _logger.LogWarning("Produit avec ID {Id} non trouvé.", id);
                return NotFound(new { message = $"Produit ID={id} non trouvé." });
            }

            _logger.LogInformation("Produit ID {Id} trouvé : {Nom}", p.ProduitId, p.Nom);
            return Ok(p);
        }

        /// <summary>
        /// Créer un nouveau produit.
        /// </summary>
        /// <param name="produit"></param>
        /// <returns></returns>
        // POST: api/v1/produits
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] Produit produit)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Création échouée. Modèle invalide : {@Produit}", produit);
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Création d’un nouveau produit : {@Produit}", produit);
            var created = await _produitService.CreateAsync(produit);
            _logger.LogInformation("Produit créé avec succès : ID {Id}", created.ProduitId);
            return CreatedAtAction(nameof(GetById), new { id = created.ProduitId }, created);
        }

        /// <summary>
        /// Mettre à jour un produit existant.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="produit"></param>
        /// <returns></returns>
        // PUT: api/v1/produits/{id}
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] Produit produit)
        {
            if (id != produit.ProduitId)
            {
                _logger.LogWarning("ID dans l’URL ({UrlId}) ne correspond pas à l’ID du produit ({BodyId}).", id, produit.ProduitId);
                return BadRequest(new { message = "ID dans l’URL diffère de l’ID du corps." });
            }

            try
            {
                _logger.LogInformation("Mise à jour du produit ID {Id}.", id);
                await _produitService.UpdateAsync(produit);
                _logger.LogInformation("Produit ID {Id} mis à jour avec succès.", id);
                return NoContent();
            }
            catch (KeyNotFoundException knf)
            {
                _logger.LogWarning("Mise à jour échouée : {Message}", knf.Message);
                return NotFound(new { message = knf.Message });
            }
        }


        /// <summary>
        /// Supprimer un produit par son ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/v1/produits/{id}
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation("Suppression du produit ID {Id}.", id);
                await _produitService.DeleteAsync(id);
                _logger.LogInformation("Produit ID {Id} supprimé avec succès.", id);
                return NoContent();
            }
            catch (KeyNotFoundException knf)
            {
                _logger.LogWarning("Suppression échouée : {Message}", knf.Message);
                return NotFound(new { message = knf.Message });
            }
        }

        /// <summary>
        /// Recherche de produits par nom ou catégorie.
        /// </summary>
        /// <param name="terme">Mot-clé à rechercher.</param>
        /// <returns>Liste des produits correspondants.</returns>
        // GET: api/v1/produits/recherche?terme=xyz
        [HttpGet("recherche")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Rechercher([FromQuery] string terme)
        {
            _logger.LogInformation("Recherche de produits avec le terme : {Terme}", terme);
            var resultat = await _produitService.RechercherAsync(terme);
            _logger.LogInformation("produits trouvés pour le terme '{Terme}'.", terme);
            return Ok(resultat);
        }
    }
}
