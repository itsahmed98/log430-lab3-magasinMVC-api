using Microsoft.AspNetCore.Mvc;
using VenteMcService.Models;
using VenteMcService.Services;

namespace VenteMcService.Controllers
{
    [ApiController]
    [Route("api/v1/ventes")]
    public class VenteController : ControllerBase
    {
        private readonly IVenteService _venteService;
        private readonly ILogger<VenteController> _logger;

        public VenteController(IVenteService venteService, ILogger<VenteController> logger)
        {
            _venteService = venteService;
            _logger = logger;
        }

        /// <summary>
        /// Récupérer toutes les ventes.
        /// </summary>
        /// <returns></returns>
        // GET: api/v1/ventes
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var list = await _venteService.GetAllAsync();
            return Ok(list);
        }

        /// <summary>
        /// Recupérer une vente par son identifiant.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/v1/ventes/{id}
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var v = await _venteService.GetByIdAsync(id);
            if (v == null)
                return NotFound(new { message = $"Vente ID={id} non trouvé." });
            return Ok(v);
        }

        // GET: api/v1/ventes/magasin/{magasinId}
        [HttpGet("magasin/{magasinId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByMagasin(int magasinId)
        {
            var list = await _venteService.GetByMagasinAsync(magasinId);
            return Ok(list);
        }

        // POST: api/v1/ventes
        /// <summary>
        /// Créer une vente, avec ses lignes dans le corps JSON.
        /// Exemple de JSON :
        /// {
        ///   "MagasinId": 1,
        ///   "Date": "2025-06-18T12:00:00Z",
        ///   "Lignes": [
        ///     { "ProduitId": 1, "Quantite": 2, "PrixUnitaire": 1.50 },
        ///     { "ProduitId": 3, "Quantite": 5, "PrixUnitaire": 12.00 }
        ///   ]
        /// }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] Vente vente)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _venteService.CreateAsync(vente);
                return CreatedAtAction(nameof(GetById), new { id = created.VenteId }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de la vente.");
                // Si validation métier échoue, on peut renvoyer BadRequest avec message.
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Supprimer une vente par son identifiant.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/v1/ventes/{id}
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _venteService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException knf)
            {
                return NotFound(new { message = knf.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de la vente.");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
