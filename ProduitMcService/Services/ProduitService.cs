using Microsoft.EntityFrameworkCore;
using ProduitMcService.Data;
using ProduitMcService.Models;

namespace ProduitMcService.Services
{
    /// <summary>
    /// Implémentation du service gestion produit.
    /// </summary>
    public class ProduitService : IProduitService
    {
        private readonly ProduitDbContext _context;
        private readonly ILogger<ProduitService> _logger;

        public ProduitService(ILogger<ProduitService> logger, ProduitDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<List<Produit>> GetAllAsync()
        {
            _logger.LogInformation("Récupération de tous les produits.");
            return await _context.Produits.AsNoTracking().ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<Produit?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Récupération du produit avec ID {Id}.", id);
            var produit = await _context.Produits.FindAsync(id);

            if (produit == null)
            {
                _logger.LogWarning("Produit avec ID {Id} non trouvé.", id);
            }

            return produit;
        }

        /// <inheritdoc/>
        public async Task<Produit> CreateAsync(Produit produit)
        {
            _logger.LogInformation("Création d’un nouveau produit : {@Produit}.", produit);
            _context.Produits.Add(produit);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Produit créé avec ID {Id}.", produit.ProduitId);
            return produit;
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(Produit produit)
        {
            _logger.LogInformation("Mise à jour du produit avec ID {Id}.", produit.ProduitId);
            var exist = await _context.Produits.AsNoTracking().FirstOrDefaultAsync(p => p.ProduitId == produit.ProduitId);

            if (exist == null)
            {
                _logger.LogWarning("Échec de la mise à jour : produit avec ID {Id} introuvable.", produit.ProduitId);
                throw new KeyNotFoundException($"Produit ID {produit.ProduitId} introuvable.");
            }

            _context.Produits.Update(produit);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Produit avec ID {Id} mis à jour avec succès.", produit.ProduitId);
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation("Suppression du produit avec ID {Id}.", id);
            var ent = await _context.Produits.FindAsync(id);

            if (ent == null)
            {
                _logger.LogWarning("Échec de la suppression : produit avec ID {Id} introuvable.", id);
                throw new KeyNotFoundException($"Produit ID {id} introuvable.");
            }

            _context.Produits.Remove(ent);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Produit avec ID {Id} supprimé avec succès.", id);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Produit>> RechercherAsync(string terme)
        {
            terme = terme.ToLower();

            return await _context.Produits
                .Where(p =>
                    p.Nom.ToLower().Contains(terme) ||
                    (p.Categorie != null && p.Categorie.ToLower().Contains(terme)))
                .ToListAsync();
        }
    }
}
