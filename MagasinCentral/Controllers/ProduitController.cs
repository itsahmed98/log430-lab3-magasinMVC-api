using MagasinCentral.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace MagasinCentral.Controllers
{
    /// <summary>
    /// Contrôleur pour les performances du tableau de bord (UC3).
    /// </summary>
    public class ProduitController : Controller
    {
        private readonly ILogger<ProduitController> _logger;
        private readonly HttpClient _httpClient;

        public ProduitController(ILogger<ProduitController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClientFactory?.CreateClient("ProduitMcService") ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        /// <summary>
        /// Afficher la liste des produits disponibles.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Requête de récupération de la liste des produits envoyée.");
            var response = await _httpClient.GetAsync("");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Échec de la récupération des produits. Code HTTP : {StatusCode}", response.StatusCode);
                return View("Error");
            }

            var content = await response.Content.ReadAsStringAsync();
            var produits = JsonSerializer.Deserialize<List<ProduitDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _logger.LogInformation("{Count} produits récupérés avec succès.", produits?.Count ?? 0);
            return View(produits);
        }

        /// <summary>
        /// Modifier un produit existant.
        /// </summary>
        /// <param name="produitId"></param>
        public async Task<IActionResult> Modifier(int produitId)
        {
            _logger.LogInformation("Requête pour modifier le produit ID : {ProduitId}", produitId);
            var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/{produitId}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Produit ID {ProduitId} non trouvé. Code HTTP : {StatusCode}", produitId, response.StatusCode);
                return NotFound();
            }

            var content = await response.Content.ReadAsStringAsync();
            var produit = JsonSerializer.Deserialize<ProduitDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _logger.LogInformation("Produit ID {ProduitId} récupéré pour modification.", produitId);
            return View(produit);
        }

        /// <summary>
        /// Modifier un produit existant avec les données du formulaire.
        /// </summary>
        /// <param name="produit"></param>
        [HttpPost]
        public async Task<IActionResult> Modifier(ProduitDto produit)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Échec de validation du formulaire pour le produit ID : {ProduitId}", produit.ProduitId);
                return View(produit);
            }

            _logger.LogInformation("Envoi des données modifiées pour le produit ID : {ProduitId}", produit.ProduitId);
            var json = JsonSerializer.Serialize(produit);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/{produit.ProduitId}", content);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Échec de la mise à jour du produit ID : {ProduitId}. Code HTTP : {StatusCode}", produit.ProduitId, response.StatusCode);
                return View("Error");
            }

            _logger.LogInformation("Produit ID : {ProduitId} mis à jour avec succès.", produit.ProduitId);
            TempData["Succès"] = $"Le produit « {produit.Nom} » a bien été mis à jour.";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Recherche de produits par identifiant, nom ou catégorie.
        /// </summary>
        /// <param name="produit"></param>
        /// <returns></returns>
        public async Task<IActionResult> Recherche(string produit)
        {
            _logger.LogInformation("Recherche de produits avec le terme : {Terme}", produit);
            var searchResponse = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/recherche?terme={produit}");

            if (!searchResponse.IsSuccessStatusCode)
            {
                _logger.LogWarning("La recherche pour le terme {Terme} a échoué. Code HTTP : {StatusCode}", produit, searchResponse.StatusCode);
                return View(new List<ProduitDto>());
            }

            var searchContent = await searchResponse.Content.ReadAsStringAsync();
            var results = JsonSerializer.Deserialize<List<ProduitDto>>(searchContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _logger.LogInformation("{Count} résultats trouvés pour le terme : {Terme}", results?.Count ?? 0, produit);
            return View(results);
        }
    }
}
