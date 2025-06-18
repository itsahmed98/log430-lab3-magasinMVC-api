using Microsoft.AspNetCore.Mvc;
using ProduitMcService.Models;
using ProduitMcService.Services;

namespace ProduitMcService.Controllers
{
    [ApiController]
    [Route("api/v1/produits")]
    public class ProduitController : ControllerBase
    {
        private readonly IProduitService _produitService;
        private readonly ILogger<ProduitController> _logger;

        public ProduitController(IProduitService produitService, ILogger<ProduitController> logger)
        {
            _produitService = produitService;
            _logger = logger;
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
            var list = await _produitService.GetAllAsync();
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
            var p = await _produitService.GetByIdAsync(id);
            if (p == null) return NotFound(new { message = $"Produit ID={id} non trouvé." });
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
                return BadRequest(ModelState);

            var created = await _produitService.CreateAsync(produit);
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
                return BadRequest(new { message = "ID dans l’URL diffère de l’ID du corps." });

            try
            {
                await _produitService.UpdateAsync(produit);
                return NoContent();
            }
            catch (KeyNotFoundException knf)
            {
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
                await _produitService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException knf)
            {
                return NotFound(new { message = knf.Message });
            }
        }
    }
}
