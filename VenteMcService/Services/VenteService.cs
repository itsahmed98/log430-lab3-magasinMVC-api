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
        private readonly HttpClient _httpClient;

        public VenteService(ILogger<VenteService> logger, VenteDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClientFactory?.CreateClient("ProduitMcService") ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        /// <inheritdoc/> 
        public async Task<Vente> CreateAsync(Vente vente)
        {
            _logger.LogInformation("Début de la création d’une vente pour magasin ID {MagasinId}", vente.MagasinId);

            foreach (var ligneDto in vente.Lignes)
            {
                _logger.LogInformation("Récupération des infos produit ID {ProduitId}", ligneDto.ProduitId);
                var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/{ligneDto.ProduitId}");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Produit ID {ProduitId} non trouvé par le microservice produit.", ligneDto.ProduitId);
                    throw new Exception($"Produit {ligneDto.ProduitId} non trouvé.");
                }

                var json = await response.Content.ReadAsStringAsync();
                var produit = JsonSerializer.Deserialize<ProduitDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                vente.Lignes.Add(new LigneVente
                {
                    ProduitId = ligneDto.ProduitId,
                    Quantite = ligneDto.Quantite,
                    PrixUnitaire = produit.Prix
                });

                _logger.LogInformation("Produit ID {ProduitId} ajouté à la vente.", ligneDto.ProduitId);
            }

            _context.Ventes.Add(vente);
            await _context.SaveChangesAsync();
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
