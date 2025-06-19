using System.Net.Http.Json;
using RapportMcService.Models;

namespace RapportMcService.Services
{
    public class RapportService : IRapportService
    {
        private readonly HttpClient _httpVente;
        private readonly HttpClient _httpProduit;
        private readonly HttpClient _httpStock;

        public RapportService(IHttpClientFactory httpVente, IHttpClientFactory httpProduit, IHttpClientFactory httpStock)
        {
            _httpVente = httpVente.CreateClient("VenteMcService");
            _httpProduit = httpProduit.CreateClient("ProduitMcService");
            _httpStock = httpStock.CreateClient("StockMcService");
        }

        public async Task<IEnumerable<RapportDto>> ObtenirRapportConsolideAsync()
        {
            var ventes = await _httpVente.GetFromJsonAsync<List<VenteDto>>("");
            var produits = await _httpProduit.GetFromJsonAsync<List<ProduitDto>>("");
            var stocks = await _httpStock.GetFromJsonAsync<List<StockDto>>("");

            var rapport = ventes
                .GroupBy(v => v.MagasinId)
                .Select(g => new RapportDto
                {
                    NomMagasin = $"Magasin {g.Key}",
                    ChiffreAffairesTotal = g.Sum(v => v.Lignes.Sum(l => l.Quantite * l.PrixUnitaire)),
                    TopProduits = g
                        .SelectMany(v => v.Lignes)
                        .GroupBy(l => l.ProduitId)
                        .Select(pg => new InfosVenteProduit
                        {
                            NomProduit = produits.FirstOrDefault(p => p.ProduitId == pg.Key)?.Nom ?? $"Produit {pg.Key}",
                            QuantiteVendue = pg.Sum(l => l.Quantite),
                            TotalVentes = pg.Sum(l => l.Quantite * l.PrixUnitaire)
                        })
                        .OrderByDescending(p => p.QuantiteVendue)
                        .Take(3)
                        .ToList(),
                    StocksRestants = stocks
                        .Where(s => s.MagasinId == g.Key)
                        .Select(s => new InfosStockProduit
                        {
                            NomProduit = produits.FirstOrDefault(p => p.ProduitId == s.ProduitId)?.Nom ?? $"Produit {s.ProduitId}",
                            QuantiteRestante = s.Quantite
                        })
                        .ToList()
                })
                .ToList();

            return rapport;
        }
    }
}
