using MagasinCentral.Data;
using MagasinCentral.Models;
using MagasinCentral.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MagasinCentral.Services
{
    /// <summary>
    /// Implémentation du service métier
    /// </summary>
    public class ReapprovisionnementService : IReapprovisionnementService
    {
        private readonly MagasinDbContext _contexte;
        private readonly ILogger<ReapprovisionnementService> _logger;

        public ReapprovisionnementService(MagasinDbContext contexte, ILogger<ReapprovisionnementService> logger)
        {
            _contexte = contexte ?? throw new ArgumentNullException(nameof(contexte));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task<List<StockVue>> GetStocksAsync(int magasinId)
        {
            _logger.LogInformation("Récupération des stocks pour le magasin ID={MagasinId}", magasinId);

            var stocksLocales = await _contexte.MagasinStocksProduits
                .Where(ms => ms.MagasinId == magasinId)
                .Include(ms => ms.Produit)
                .ToListAsync();

            var stocksCentraux = await _contexte.StocksCentraux
                .Include(sc => sc.Produit)
                .ToListAsync();

            var listeStocks = new List<StockVue>();

            foreach (var stockLocal in stocksLocales)
            {
                var sc = stocksCentraux.FirstOrDefault(x => x.ProduitId == stockLocal.ProduitId);

                listeStocks.Add(new StockVue
                {
                    ProduitId = stockLocal.ProduitId,
                    NomProduit = stockLocal.Produit.Nom,
                    QuantiteLocale = stockLocal.Quantite,
                    QuantiteCentral = sc?.Quantite ?? 0
                });
            }

            _logger.LogInformation("{Count} stocks récupérés pour le magasin ID={MagasinId}", listeStocks.Count, magasinId);
            return listeStocks;
        }

        /// <inheritdoc />
        public async Task CreerDemandeReapprovisionnementAsync(int magasinId, int produitId, int quantiteDemande)
        {
            _logger.LogInformation("Création d'une demande de réapprovisionnement pour MagasinID={MagasinId}, ProduitID={ProduitId}, Quantité={Quantite}",
                magasinId, produitId, quantiteDemande);

            if (quantiteDemande <= 0)
            {
                _logger.LogWarning("Quantité demandée invalide : {Quantite}", quantiteDemande);
                throw new ArgumentException("La quantité demandée doit être strictement positive.");
            }

            var magasin = await _contexte.Magasins.FindAsync(magasinId);
            if (magasin == null)
            {
                _logger.LogWarning("Magasin non trouvé avec ID={MagasinId}", magasinId);
                throw new ArgumentException($"Le magasin d’ID={magasinId} n’existe pas.");
            }

            var produit = await _contexte.Produits.FindAsync(produitId);
            if (produit == null)
            {
                _logger.LogWarning("Produit non trouvé avec ID={ProduitId}", produitId);
                throw new ArgumentException($"Le produit d’ID={produitId} n’existe pas.");
            }

            var demande = new DemandeReapprovisionnementDto
            {
                MagasinId = magasinId,
                ProduitId = produitId,
                QuantiteDemandee = quantiteDemande,
                DateDemande = DateTime.UtcNow,
                Statut = "EnAttente"
            };

            _contexte.DemandeReapprovisionnementDtos.Add(demande);
            await _contexte.SaveChangesAsync();

            _logger.LogInformation("Demande de réapprovisionnement enregistrée avec succès (ID temporaire)");
        }

        /// <inheritdoc />
        public async Task<List<DemandeReapprovisionnementDto>> GetDemandesReapprovisionnementAsync()
        {
            _logger.LogInformation("Récupération de toutes les demandes de réapprovisionnement...");
            var result = await _contexte.DemandeReapprovisionnementDtos
                .Include(d => d.MagasinId)
                .Include(d => d.ProduitId)
                .OrderByDescending(d => d.DateDemande)
                .ToListAsync();
            _logger.LogInformation("{Count} demandes récupérées.", result.Count);
            return result;
        }

        /// <inheritdoc />
        public async Task<List<DemandeReapprovisionnementDto>> GetDemandesEnAttenteAsync()
        {
            _logger.LogInformation("Récupération des demandes en attente...");
            var result = await _contexte.DemandeReapprovisionnementDtos
                .Where(d => d.Statut == "EnAttente")
                .Include(d => d.MagasinId)
                .Include(d => d.ProduitId)
                .OrderBy(d => d.DateDemande)
                .ToListAsync();
            _logger.LogInformation("{Count} demandes en attente récupérées.", result.Count);
            return result;
        }

        /// <inheritdoc />
        public async Task TraiterDemandeAsync(int demandeId, bool approuver)
        {
            _logger.LogInformation("Traitement de la demande ID={DemandeId} (Approuver={Approuver})", demandeId, approuver);

            var demande = await _contexte.DemandeReapprovisionnementDtos
                .Include(d => d.MagasinId)
                .Include(d => d.ProduitId)
                .FirstOrDefaultAsync(d => d.DemandeId == demandeId);

            if (demande == null)
            {
                _logger.LogWarning("Demande introuvable ID={DemandeId}", demandeId);
                throw new ArgumentException($"La demande d’ID={demandeId} n’existe pas.");
            }

            if (demande.Statut != "EnAttente")
            {
                _logger.LogWarning("Demande ID={DemandeId} déjà traitée (Statut={Statut})", demandeId, demande.Statut);
                throw new InvalidOperationException("Cette demande a déjà été traitée.");
            }

            if (!approuver)
            {
                demande.Statut = "Refuse";
                await _contexte.SaveChangesAsync();
                _logger.LogInformation("Demande ID={DemandeId} refusée.", demandeId);
                return;
            }

            var stockCentral = await _contexte.StocksCentraux
                .FirstOrDefaultAsync(sc => sc.ProduitId == demande.ProduitId);

            if (stockCentral == null)
            {
                _logger.LogError("Stock central manquant pour ProduitID={ProduitId}", demande.ProduitId);
                throw new InvalidOperationException("Le stock central pour ce produit n’existe pas.");
            }

            if (stockCentral.Quantite < demande.QuantiteDemandee)
            {
                _logger.LogWarning("Stock central insuffisant pour ProduitID={ProduitId}: demandé={Demandee}, disponible={Disponible}",
                    demande.ProduitId, demande.QuantiteDemandee, stockCentral.Quantite);
                throw new InvalidOperationException("Stock central insuffisant pour approuver.");
            }

            stockCentral.Quantite -= demande.QuantiteDemandee;

            var stockLocal = await _contexte.MagasinStocksProduits
                .FirstOrDefaultAsync(ms => ms.MagasinId == demande.MagasinId && ms.ProduitId == demande.ProduitId);

            if (stockLocal == null)
            {
                stockLocal = new MagasinStockProduit
                {
                    MagasinId = demande.MagasinId,
                    ProduitId = demande.ProduitId,
                    Quantite = demande.QuantiteDemandee
                };
                _contexte.MagasinStocksProduits.Add(stockLocal);
            }
            else
            {
                stockLocal.Quantite += demande.QuantiteDemandee;
            }

            demande.Statut = "Approuve";

            await _contexte.SaveChangesAsync();
            _logger.LogInformation("Demande ID={DemandeId} approuvée avec succès.", demandeId);
        }
    }
}