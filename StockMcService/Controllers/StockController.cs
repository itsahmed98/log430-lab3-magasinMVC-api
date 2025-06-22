using Microsoft.AspNetCore.Mvc;
using StockMcService.Models;
using StockMcService.Services;

namespace StockMcService.Controllers
{
    /// <summary>
    /// Contrôleur pour gérer les demandes de réapprovisionnement des produits dans les magasins.
    /// </summary>
    [ApiController]
    [Route("api/v1/stocks")]
    public class StockController : ControllerBase
    {
        private readonly IStockService _service;
        private readonly ILogger<StockController> _logger;

        public StockController(IStockService service, ILogger<StockController> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Liste tous les stocks.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<StockDto>>> GetAll()
        {
            _logger.LogInformation("Récupération de tous les stocks.");
            var stocks = await _service.GetAllStocksAsync();
            _logger.LogInformation("{Count} stocks trouvés.", stocks.Count());
            return Ok(stocks);
        }

        /// <summary>
        /// Récupère le stock d’un produit dans un magasin.
        /// </summary>
        /// <param name="magasinId">Identifiant du magasin</param>
        /// <param name="produitId">Identifiant du produit</param>
        [HttpGet("{magasinId:int}/{produitId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StockDto>> GetOne(int magasinId, int produitId)
        {
            _logger.LogInformation("Recherche du stock pour MagasinId={MagasinId}, ProduitId={ProduitId}.", magasinId, produitId);

            var dto = await _service.GetStockByMagasinProduitAsync(magasinId, produitId);
            if (dto is not null)
            {
                _logger.LogInformation("Stock trouvé : {Quantite} unités disponibles.", dto.Quantite);
                return Ok(dto);
            }

            _logger.LogWarning("Stock non trouvé pour MagasinId={MagasinId}, ProduitId={ProduitId}.", magasinId, produitId);
            return NotFound(new { message = $"Stock introuvable pour magasin #{magasinId} et produit #{produitId}." });
        }
    }
}
