using Microsoft.AspNetCore.Mvc;
using StockMcService.Models;
using StockMcService.Services;

namespace StockMcService.Controllers
{
    [ApiController]
    [Route("api/v1/stocks")]
    public class StockController : ControllerBase
    {
        private readonly IStockService _service;
        public StockController(IStockService service) => _service = service;

        /// <summary>Liste tous les stocks.</summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockDto>>> GetAll()
            => Ok(await _service.GetAllStocksAsync());

        /// <summary>Stock d’un produit dans un magasin.</summary>
        [HttpGet("{magasinId:int}/{produitId:int}")]
        public async Task<ActionResult<StockDto>> GetOne(int magasinId, int produitId)
        {
            var dto = await _service.GetStockByMagasinProduitAsync(magasinId, produitId);
            return dto is not null ? Ok(dto) : NotFound();
        }
    }
}
