using MagasinCentral.Models;
using Microsoft.AspNetCore.Mvc;

namespace MagasinCentral.Api.Controllers
{
    /// <summary>
    /// Contrôleur pour les rapports consolidés des ventes.
    /// </summary>
    [ApiController]
    [Route("api/v1/rapports")]
    //[Authorize]
    public class RapportApiController : ControllerBase
    {
        private readonly ILogger<RapportApiController> _logger;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Constructeur du contrôleur de rapport.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="rapportService"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public RapportApiController(ILogger<RapportApiController> logger, IHttpClientFactory httpClientFactory)
        {
            {
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _httpClient = httpClientFactory?.CreateClient("RapportMcService") ?? throw new ArgumentNullException(nameof(httpClientFactory));
            }
        }

        /// <summary>
        /// Récupérer le rapport consolidé des ventes.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(RapportDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RapportConsolide()
        {
            IActionResult result = null!;

            try
            {
                result = Ok(await _httpClient.GetFromJsonAsync<RapportVentesDto>(""));
                _logger.LogInformation("Rapport consolidé récupéré avec succès.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du rapport consolidé.");
                result = StatusCode(500, "Une erreur s'est produite lors de la récupération du rapport.");
            }

            return result;
        }
    }
}