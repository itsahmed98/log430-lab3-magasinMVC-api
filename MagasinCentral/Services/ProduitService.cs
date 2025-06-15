using MagasinCentral.Data;
using MagasinCentral.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace MagasinCentral.Services
{
    /// <summary>
    /// Service pour gérer les opérations liées aux produits.
    /// </summary>
    public class ProduitService : IProduitService
    {
        private readonly MagasinDbContext _contexte;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ProduitService> _logger;

        public ProduitService(IMemoryCache cache, MagasinDbContext contexte, ILogger<ProduitService> logger)
        {
            _contexte = contexte ?? throw new ArgumentNullException(nameof(contexte));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task<List<Produit>> GetAllProduitsAsync()
        {
            _logger.LogInformation("Récupération de tous les produits...");
            var produits = await _contexte.Produits
                .AsNoTracking()
                .ToListAsync();
            _logger.LogInformation("{Count} produits récupérés.", produits.Count);
            return produits;
        }

        /// <inheritdoc />
        public async Task<Produit?> GetProduitByIdAsync(int produitId)
        {
            string key = $"produit_{produitId}";
            _logger.LogInformation("Recherche du produit ID={ProduitId} dans le cache...", produitId);

            return await _cache.GetOrCreateAsync(key, async entry =>
            {
                _logger.LogInformation("Produit ID={ProduitId} non trouvé dans le cache. Lecture depuis la base de données...", produitId);
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);

                var produit = await _contexte.Produits
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.ProduitId == produitId);

                if (produit == null)
                    _logger.LogWarning("Aucun produit trouvé avec l'ID={ProduitId}.", produitId);
                else
                    _logger.LogInformation("Produit ID={ProduitId} récupéré depuis la base.", produitId);

                return produit;
            });
        }

        /// <inheritdoc />
        public async Task ModifierProduitAsync(Produit produit)
        {
            _logger.LogInformation("Modification du produit ID={ProduitId}.", produit.ProduitId);

            var exist = await _contexte.Produits
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProduitId == produit.ProduitId);

            if (exist == null)
            {
                _logger.LogWarning("Tentative de modification d'un produit inexistant ID={ProduitId}.", produit.ProduitId);
                throw new ArgumentException($"Le produit d’ID={produit.ProduitId} n’existe pas.");
            }

            _contexte.Produits.Update(produit);
            await _contexte.SaveChangesAsync();
            _cache.Remove($"produit_{produit.ProduitId}");
            _logger.LogInformation("Produit ID={ProduitId} modifié et cache invalidé.", produit.ProduitId);
        }

        /// <inheritdoc />
        public async Task<List<Produit>> RechercherProduitsAsync(string terme)
        {
            terme = terme?.Trim().ToLower() ?? "";
            _logger.LogInformation("Recherche de produits avec le terme: '{Terme}'", terme);

            var resultats = await _contexte.Produits
                .AsNoTracking()
                .Where(p =>
                    p.ProduitId.ToString() == terme ||
                    p.Nom.ToLower().Contains(terme) ||
                    (p.Categorie != null && p.Categorie.ToLower().Contains(terme))
                )
                .ToListAsync();

            _logger.LogInformation("{Count} produits trouvés pour le terme '{Terme}'.", resultats.Count, terme);
            return resultats;
        }
    }
}
