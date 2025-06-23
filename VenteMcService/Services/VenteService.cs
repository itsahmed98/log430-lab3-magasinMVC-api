using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using VenteMcService.Data;
using VenteMcService.Models;

namespace VenteMcService.Services
{
    public class VenteService : IVenteService
    {
        private readonly VenteDbContext _context;
        private readonly ILogger<VenteService> _logger;
        private readonly HttpClient _httpProduit;
        private readonly HttpClient _httpStock;

        public VenteService(ILogger<VenteService> logger, VenteDbContext context, IHttpClientFactory httpClientProduit, IHttpClientFactory httpClientStock)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpProduit = httpClientProduit?.CreateClient("ProduitMcService") ?? throw new ArgumentNullException(nameof(httpClientProduit));
            _httpStock = httpClientStock?.CreateClient("StockMcService") ?? throw new ArgumentNullException(nameof(httpClientStock));
        }

        /// <inheritdoc/> 
        public async Task<Vente> CreateAsync(Vente vente)
        {
            _logger.LogInformation("Début de la création d’une vente pour magasin ID {MagasinId}", vente.MagasinId);

            var nouvellesLignes = new List<LigneVente>();

            foreach (var ligneDto in vente.Lignes.ToList())
            {
                _logger.LogInformation("Récupération des infos produit ID {ProduitId}", ligneDto.ProduitId);

                ProduitDto? produit;
                try
                {
                    produit = await _httpProduit.GetFromJsonAsync<ProduitDto>($"{_httpProduit.BaseAddress}/{ligneDto.ProduitId}");
                    if (produit == null)
                    {
                        _logger.LogWarning("Produit ID {ProduitId} non trouvé par le microservice produit.", ligneDto.ProduitId);
                        throw new Exception($"Produit {ligneDto.ProduitId} non trouvé.");
                    }
                    _logger.LogInformation("Produit ID {ProduitId} récupéré avec succès pour créer une vente.", ligneDto.ProduitId);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "Erreur lors de la récupération du produit ID {ProduitId} pour créer une vente", ligneDto.ProduitId);
                    throw new Exception($"Erreur lors de la récupération du produit {ligneDto.ProduitId} : {ex.Message}");
                }

                nouvellesLignes.Add(new LigneVente
                {
                    ProduitId = ligneDto.ProduitId,
                    Quantite = ligneDto.Quantite,
                    PrixUnitaire = produit.Prix
                });

                _logger.LogInformation("Produit ID {ProduitId} ajouté à la vente.", ligneDto.ProduitId);
            }

            vente.Lignes = nouvellesLignes;

            _context.Ventes.Add(vente);
            await _context.SaveChangesAsync();

            // Mettre a jour le stock pour chaque ligne de vente
            foreach (var ligne in nouvellesLignes)
            {
                _logger.LogInformation("Mise à jour du stock pour le produit ID {ProduitId} avec la quantité {Quantite}", ligne.ProduitId, ligne.Quantite);
                try
                {
                    //https://localhost:7185/api/v1/stocks?magasinId=2&produitId=3&quantite=3
                    var response = await _httpStock.PutAsync($"{_httpStock.BaseAddress}?magasinId={vente.MagasinId}&produitId={ligne.ProduitId}&quantite={-ligne.Quantite}", null);
                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogError("Erreur lors de la mise à jour du stock pour le produit ID {ProduitId}.", ligne.ProduitId);
                        throw new Exception($"Erreur lors de la mise à jour du stock pour le produit {ligne.ProduitId}.");
                    }
                    _logger.LogInformation("Stock mis à jour avec succès pour le produit ID {ProduitId}.", ligne.ProduitId);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "Erreur lors de la mise à jour du stock pour le produit ID {ProduitId}", ligne.ProduitId);
                    throw new Exception($"Erreur lors de la mise à jour du stock pour le produit {ligne.ProduitId} : {ex.Message}");
                }
            }

            _logger.LogInformation("Vente enregistrée avec ID {VenteId}", vente.VenteId);
            return vente;
        }

        /// <inheritdoc/> 
        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation("Tentative de suppression de la vente ID {Id}", id);
            var exist = await _context.Ventes.Include(v => v.Lignes).FirstOrDefaultAsync(v => v.VenteId == id);

            if (exist == null)
            {
                _logger.LogWarning("Vente ID {Id} introuvable pour suppression.", id);
                throw new KeyNotFoundException($"Vente ID={id} introuvable.");
            }

            _context.LignesVente.RemoveRange(exist.Lignes);
            _context.Ventes.Remove(exist);
            await _context.SaveChangesAsync();

            foreach (var ligne in exist.Lignes)
            {
                _logger.LogInformation("Mise à jour du stock pour le produit ID {ProduitId} avec la quantité {Quantite}", ligne.ProduitId, ligne.Quantite);
                try
                {
                    //https://localhost:7185/api/v1/stocks?magasinId=2&produitId=3&quantite=3
                    var response = await _httpStock.PutAsync($"{_httpStock.BaseAddress}?magasinId={exist.MagasinId}&produitId={ligne.ProduitId}&quantite={ligne.Quantite}", null);
                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogError("Erreur lors de la mise à jour du stock pour le produit ID {ProduitId}.", ligne.ProduitId);
                        throw new Exception($"Erreur lors de la mise à jour du stock pour le produit {ligne.ProduitId}.");
                    }
                    _logger.LogInformation("Stock mis à jour avec succès pour le produit ID {ProduitId}.", ligne.ProduitId);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "Erreur lors de la mise à jour du stock pour le produit ID {ProduitId}", ligne.ProduitId);
                    throw new Exception($"Erreur lors de la mise à jour du stock pour le produit {ligne.ProduitId} : {ex.Message}");
                }
            }

            _logger.LogInformation("Vente ID {Id} supprimée avec succès.", id);
        }

        /// <inheritdoc/> 
        public async Task<List<Vente>> GetAllAsync()
        {
            _logger.LogInformation("Récupération de toutes les ventes.");
            var ventes = await _context.Ventes
                .Include(v => v.Lignes)
                .AsNoTracking()
                .ToListAsync();
            _logger.LogInformation("{Count} ventes récupérées.", ventes.Count);
            return ventes;
        }

        /// <inheritdoc/> 
        public async Task<Vente?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Recherche de la vente ID {Id}", id);
            var vente = await _context.Ventes
                .Include(v => v.Lignes)
                .FirstOrDefaultAsync(v => v.VenteId == id);

            if (vente == null)
                _logger.LogWarning("Vente ID {Id} non trouvée.", id);
            else
                _logger.LogInformation("Vente ID {Id} trouvée.", id);

            return vente;
        }

        /// <inheritdoc/>
        public async Task<List<Vente>> GetByMagasinAsync(int magasinId)
        {
            _logger.LogInformation("Récupération des ventes pour le magasin ID {MagasinId}", magasinId);
            var ventes = await _context.Ventes
                .Where(v => v.MagasinId == magasinId)
                .Include(v => v.Lignes)
                .AsNoTracking()
                .ToListAsync();
            _logger.LogInformation("{Count} ventes récupérées pour magasin ID {MagasinId}", ventes.Count, magasinId);
            return ventes;
        }
    }
}
