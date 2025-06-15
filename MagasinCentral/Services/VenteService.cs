using MagasinCentral.Models;
using MagasinCentral.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MagasinCentral.Services
{
    /// <summary>
    /// Service pour gérer les opérations liées aux ventes.
    /// </summary>
    public class VenteService : IVenteService
    {
        private readonly MagasinDbContext _contexte;
        private readonly ILogger<VenteService> _logger;

        public VenteService(MagasinDbContext contexte, ILogger<VenteService> logger)
        {
            _contexte = contexte ?? throw new ArgumentNullException(nameof(contexte));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task<int> CreerVenteAsync(int magasinId, List<(int produitId, int quantite)> lignes)
        {
            _logger.LogInformation("Création d'une vente pour le magasin ID={MagasinId} avec {NbLignes} lignes.", magasinId, lignes.Count);

            if (!lignes.Any())
            {
                _logger.LogWarning("Tentative de créer une vente avec aucune ligne.");
                throw new ArgumentException("Aucune ligne.");
            }

            var vente = new Vente { MagasinId = magasinId, Date = DateTime.UtcNow };

            foreach (var (pid, q) in lignes.Where(x => x.quantite > 0))
            {
                var prod = await _contexte.Produits.FindAsync(pid);
                if (prod == null)
                {
                    _logger.LogWarning("Produit ID={ProduitId} introuvable.", pid);
                    throw new ArgumentException($"Produit {pid} invalide");
                }

                vente.Lignes.Add(new LigneVente
                {
                    ProduitId = pid,
                    Quantite = q,
                    PrixUnitaire = prod.Prix
                });

                var stockLocal = await _contexte.MagasinStocksProduits
                    .FirstOrDefaultAsync(ms => ms.MagasinId == magasinId && ms.ProduitId == pid);

                if (stockLocal == null)
                {
                    _logger.LogInformation("Aucun stock local trouvé pour ProduitID={ProduitId}, MagasinID={MagasinId}. Initialisation à 0.", pid, magasinId);
                    stockLocal = new MagasinStockProduit
                    {
                        MagasinId = magasinId,
                        ProduitId = pid,
                        Quantite = 0
                    };
                    _contexte.MagasinStocksProduits.Add(stockLocal);
                }

                if (stockLocal.Quantite < q)
                {
                    _logger.LogWarning("Stock insuffisant pour ProduitID={ProduitId}, MagasinID={MagasinId}. Requis={Requis}, Disponible={Dispo}", pid, magasinId, q, stockLocal.Quantite);
                    throw new InvalidOperationException($"Stock insuffisant pour le produit {pid} dans le magasin {magasinId}.");
                }

                stockLocal.Quantite -= q;
                _logger.LogInformation("Stock mis à jour pour ProduitID={ProduitId} : nouvelle quantité = {NouvelleQuantite}", pid, stockLocal.Quantite);
            }

            _contexte.Ventes.Add(vente);
            await _contexte.SaveChangesAsync();
            _logger.LogInformation("Vente créée avec succès (ID={VenteId})", vente.VenteId);

            return vente.VenteId;
        }

        /// <inheritdoc />
        public async Task AnnulerVenteAsync(int venteId)
        {
            _logger.LogInformation("Annulation de la vente ID={VenteId}", venteId);

            var vente = await _contexte.Ventes
                .Include(v => v.Lignes)
                .FirstOrDefaultAsync(v => v.VenteId == venteId);

            if (vente == null)
            {
                _logger.LogWarning("Vente ID={VenteId} introuvable pour annulation.", venteId);
                throw new ArgumentException("Vente introuvable.");
            }

            foreach (var l in vente.Lignes)
            {
                var stock = await _contexte.MagasinStocksProduits
                    .FirstOrDefaultAsync(ms =>
                        ms.MagasinId == vente.MagasinId &&
                        ms.ProduitId == l.ProduitId);

                if (stock != null)
                {
                    stock.Quantite += l.Quantite;
                    _logger.LogInformation("Quantité restituée pour ProduitID={ProduitId} : nouvelle quantité = {Quantite}", l.ProduitId, stock.Quantite);
                }
                else
                {
                    _logger.LogWarning("Stock introuvable pour ProduitID={ProduitId} lors de l'annulation.", l.ProduitId);
                }
            }

            _contexte.Ventes.Remove(vente);
            await _contexte.SaveChangesAsync();

            _logger.LogInformation("Vente ID={VenteId} annulée avec succès.", venteId);
        }

        /// <inheritdoc />
        public async Task<List<Vente>> GetVentesAsync()
        {
            _logger.LogInformation("Chargement de toutes les ventes...");
            var ventes = await _contexte.Ventes
                .Include(v => v.Magasin)
                .Include(v => v.Lignes)
                    .ThenInclude(l => l.Produit)
                .OrderByDescending(v => v.Date)
                .ToListAsync();

            _logger.LogInformation("{Count} ventes récupérées.", ventes.Count);
            return ventes;
        }
    }
}
