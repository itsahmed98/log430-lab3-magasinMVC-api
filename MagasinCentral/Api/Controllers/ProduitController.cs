using Microsoft.AspNetCore.Mvc;
using MagasinCentral.Services;

namespace MagasinCentral.Api.Controllers
{
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

        [HttpGet]
        public async Task<IActionResult> Produits()
        {
            return Ok(await _produitService.GetAllProduitsAsync());
        }

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

        [HttpPut("{produitId:int}")]
        public async Task<IActionResult> Modifier(int produitId)
        {
            var produit = await _produitService.GetProduitByIdAsync(produitId);
            if (produit == null)
            {
                return NotFound();
            }
            return Ok(produit);
        }
    }
}

