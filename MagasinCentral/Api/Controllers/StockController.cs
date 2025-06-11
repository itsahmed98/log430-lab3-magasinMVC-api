using MagasinCentral.Models;
using MagasinCentral.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MagasinCentral.Api.Controllers
{
    /// <summary>
    /// Contrôleur pour le stock des magasins.
    /// </summary>
    [ApiController]
    [Route("api/v1/stocks")]
    [Authorize]
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;
        private readonly ILogger<StockController> _logger;

        /// <summary>
        /// Constructeur du contrôleur de rapport.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="stockService"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public StockController(ILogger<StockController> logger, IStockService stockService)
        {
            {
                _stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }
        }

        /// <summary>
        /// Récupérer la quantité du stock dans un magasin spécifique.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> GetStockMagasin(int magasinId)
        {
            try
            {
                int? quantite = await _stockService.GetStockByMagasinId(magasinId);

                if (quantite == null)
                    return NotFound();

                return quantite.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du stock du magasin: {MagasinId}", magasinId);
                return StatusCode(500, "Une erreur s'est produite lors de la récupération du stock.");
            }
        }
    }
}