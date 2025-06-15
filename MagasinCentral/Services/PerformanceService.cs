using MagasinCentral.Data;
using MagasinCentral.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace MagasinCentral.Services
{
    /// <summary>
    /// Service pour gérer les performances du tableau de bord.
    /// </summary>
    public class PerformancesService : IPerformancesService
    {
        private readonly MagasinDbContext _contexte;
        private readonly ILogger<PerformancesService> _logger;

        /// <summary>
        /// Seuil de surstock local.
        /// </summary>
        private const int seuilSurstock = 100;

        public PerformancesService(ILogger<PerformancesService> logger, MagasinDbContext contexte)
        {
            _contexte = contexte ?? throw new ArgumentNullException(nameof(contexte));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task<PerformancesViewModel> GetPerformances()
        {
            _logger.LogInformation("Début du calcul des performances du tableau de bord.");

            var viewModel = new PerformancesViewModel();

            try
            {
                _logger.LogInformation("Chargement de la liste des magasins...");
                var magasins = await _contexte.Magasins
                    .AsNoTracking()
                    .ToListAsync();
                _logger.LogInformation("{Count} magasins chargés.", magasins.Count);

                _logger.LogInformation("Calcul du chiffre d'affaires par magasin...");
                var ventesParMagasin = await _contexte.LignesVente
                    .AsNoTracking()
                    .GroupBy(l => l.Vente.MagasinId)
                    .Select(g => new
                    {
                        MagasinId = g.Key,
                        ChiffreAffaires = g.Sum(l => l.PrixUnitaire * l.Quantite)
                    })
                    .ToListAsync();

                foreach (var magasin in magasins)
                {
                    var infoVente = ventesParMagasin
                        .FirstOrDefault(x => x.MagasinId == magasin.MagasinId);

                    viewModel.RevenusParMagasin.Add(new RevenuMagasin
                    {
                        MagasinId = magasin.MagasinId,
                        NomMagasin = magasin.Nom,
                        ChiffreAffaires = infoVente?.ChiffreAffaires ?? 0m
                    });
                }
                _logger.LogInformation("Chiffre d'affaires par magasin calculé.");

                _logger.LogInformation("Récupération des produits en rupture de stock...");
                var ruptures = await _contexte.MagasinStocksProduits
                    .AsNoTracking()
                    .Where(ms => ms.Quantite == 0)
                    .Include(ms => ms.Magasin)
                    .Include(ms => ms.Produit)
                    .ToListAsync();
                _logger.LogInformation("{Count} produits en rupture de stock trouvés.", ruptures.Count);

                foreach (var stock in ruptures)
                {
                    viewModel.ProduitsRupture.Add(new StockProduitLocal
                    {
                        MagasinId = stock.MagasinId,
                        NomMagasin = stock.Magasin.Nom,
                        ProduitId = stock.ProduitId,
                        NomProduit = stock.Produit.Nom,
                        QuantiteLocale = stock.Quantite
                    });
                }

                _logger.LogInformation("Récupération des produits en surstock local...");
                var surstocks = await _contexte.MagasinStocksProduits
                    .AsNoTracking()
                    .Where(ms => ms.Quantite > seuilSurstock)
                    .Include(ms => ms.Magasin)
                    .Include(ms => ms.Produit)
                    .ToListAsync();
                _logger.LogInformation("{Count} produits en surstock trouvés.", surstocks.Count);

                foreach (var stock in surstocks)
                {
                    viewModel.ProduitsSurstock.Add(new StockProduitLocal
                    {
                        MagasinId = stock.MagasinId,
                        NomMagasin = stock.Magasin.Nom,
                        ProduitId = stock.ProduitId,
                        NomProduit = stock.Produit.Nom,
                        QuantiteLocale = stock.Quantite
                    });
                }

                _logger.LogInformation("Calcul des tendances hebdomadaires...");
                DateTime aujourdHui = DateTime.UtcNow.Date;
                DateTime semainePasse = aujourdHui.AddDays(-6);

                var ventesDerniereSemaine = await _contexte.LignesVente
                    .AsNoTracking()
                    .Include(l => l.Vente)
                    .Where(l =>
                        l.Vente.Date >= semainePasse &&
                        l.Vente.Date < aujourdHui.AddDays(1))
                    .ToListAsync();
                _logger.LogInformation("{Count} lignes de vente récupérées pour la dernière semaine.", ventesDerniereSemaine.Count);

                var regroupement = ventesDerniereSemaine
                    .GroupBy(l => new
                    {
                        PharmacyId = l.Vente.MagasinId,
                        Jour = l.Vente.Date.Date
                    })
                    .Select(g => new
                    {
                        MagasinId = g.Key.PharmacyId,
                        Jour = g.Key.Jour,
                        MontantTotal = g.Sum(l => l.PrixUnitaire * l.Quantite)
                    })
                    .ToList();

                foreach (var magasin in magasins)
                {
                    var listeVentesJournalières = new List<VentesQuotidiennes>();

                    for (int offset = 0; offset < 7; offset++)
                    {
                        var dateCible = semainePasse.AddDays(offset);
                        var element = regroupement
                            .FirstOrDefault(x => x.MagasinId == magasin.MagasinId
                                            && x.Jour == dateCible);

                        decimal montant = element?.MontantTotal ?? 0m;

                        listeVentesJournalières.Add(new VentesQuotidiennes
                        {
                            Date = dateCible,
                            MontantVentes = montant
                        });
                    }

                    viewModel.TendancesHebdomadairesParMagasin.Add(
                        magasin.MagasinId,
                        listeVentesJournalières);
                }

                _logger.LogInformation("Tendances hebdomadaires calculées avec succès.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du calcul des performances.");
                throw;
            }

            return viewModel;
        }
    }
}
