using RapportMcService.Models;

namespace RapportMcService.Services
{
    /// <summary>
    /// Fournit les fonctionnalités pour la gestion des rapports consolidés des ventes.
    /// </summary>
    public class RapportService : IRapportService
    {
        private readonly ILogger<RapportService> _logger;
        private readonly HttpClient _httpVente;
        private readonly HttpClient _httpProduit;
        private readonly HttpClient _httpStock;

        public RapportService(
            ILogger<RapportService> logger,
            IHttpClientFactory httpVente,
            IHttpClientFactory httpProduit,
            IHttpClientFactory httpStock)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpVente = httpVente.CreateClient("VenteMcService");
            _httpProduit = httpProduit.CreateClient("ProduitMcService");
            _httpStock = httpStock.CreateClient("StockMcService");
        }

        /// <summary>
        /// Retourne les rapports consolidés des ventes.
        /// </summary>
        /// <returns>Un DTO contenant les informations du rapport.</returns>
        public async Task<RapportVentesDto> ObtenirRapportConsolideAsync()
        {
            _logger.LogInformation("Début de la génération du rapport consolidé des ventes.");

            try
            {
                _logger.LogInformation("Récupération des ventes...");
                var ventes = await _httpVente.GetFromJsonAsync<List<VenteDto>>("");
                if (ventes == null)
                {
                    _logger.LogWarning("Aucune vente récupérée depuis le microservice Vente.");
                    ventes = new List<VenteDto>();
                }

                _logger.LogInformation("Récupération des produits...");
                var produits = await _httpProduit.GetFromJsonAsync<List<ProduitDto>>("");
                if (produits == null)
                {
                    _logger.LogWarning("Aucun produit récupéré depuis le microservice Produit.");
                    produits = new List<ProduitDto>();
                }

                _logger.LogInformation("Récupération des stocks...");
                var stocks = await _httpStock.GetFromJsonAsync<List<StockDto>>("");
                if (stocks == null)
                {
                    _logger.LogWarning("Aucun stock récupéré depuis le microservice Stock.");
                    stocks = new List<StockDto>();
                }

                _logger.LogInformation("Calcul des ventes par magasin...");
                var ventesParMagasin = ventes
                    .GroupBy(v => v.MagasinId)
                    .Select(g => new VenteParMagasinDto
                    {
                        MagasinId = g.Key,
                        TotalVentes = g.Sum(v => v.Lignes.Sum(l => l.PrixUnitaire * l.Quantite)),
                        NombreTransactions = g.Count()
                    })
                    .ToList();

                _logger.LogInformation("Détermination des produits les plus vendus...");
                var produitsLesPlusVendus = ventes
                    .SelectMany(v => v.Lignes)
                    .GroupBy(l => l.ProduitId)
                    .Select(g => new ProduitLePlusVenduDto
                    {
                        ProduitId = g.Key,
                        QuantiteTotaleVendue = g.Sum(l => l.Quantite),
                        Nom = produits.FirstOrDefault(p => p.ProduitId == g.Key)?.Nom ?? $"Produit {g.Key}"
                    })
                    .OrderByDescending(p => p.QuantiteTotaleVendue)
                    .Take(10)
                    .ToList();

                _logger.LogInformation("Préparation des stocks restants...");
                var stocksRestants = stocks
                    .Select(s => new StockDto
                    {
                        MagasinId = s.MagasinId,
                        ProduitId = s.ProduitId,
                        NomProduit = produits.FirstOrDefault(p => p.ProduitId == s.ProduitId)?.Nom ?? $"Produit {s.ProduitId}",
                        Quantite = s.Quantite
                    })
                    .ToList();

                var rapport = new RapportVentesDto
                {
                    DateGeneration = DateTime.UtcNow,
                    VentesParMagasin = ventesParMagasin,
                    ProduitsLesPlusVendus = produitsLesPlusVendus,
                    StocksRestants = stocksRestants
                };

                _logger.LogInformation("Rapport consolidé généré avec succès.");
                return rapport;
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "Erreur HTTP lors de l'appel aux microservices.");
                throw new ApplicationException("Erreur de communication avec les microservices.", httpEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur inattendue lors de la génération du rapport.");
                throw new ApplicationException("Erreur lors de la génération du rapport consolidé.", ex);
            }
        }
    }
}
