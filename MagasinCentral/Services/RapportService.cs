using MagasinCentral.Data;
using MagasinCentral.Models;
using Microsoft.EntityFrameworkCore;

namespace MagasinCentral.Services
{
    /// <summary>
    /// Implémentation du service métier pour générer le rapport consolidé.
    /// </summary>
    public class RapportService : IRapportService
    {
        private readonly MagasinDbContext _contexte;
        private readonly ILogger<RapportService> _logger;

        /// <summary>
        /// Constructeur du service.
        /// </summary>
        /// <param name="contexte">Contexte EF Core.</param>
        /// <param name="logger">Logger.</param>
        public RapportService(MagasinDbContext contexte, ILogger<RapportService> logger)
        {
            _contexte = contexte ?? throw new ArgumentNullException(nameof(contexte));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task<List<RapportDto>> ObtenirRapportConsolideAsync()
        {
            _logger.LogInformation("Début de la génération du rapport consolidé...");

            _logger.LogInformation("Chargement des magasins avec ventes et stocks...");
            var listeMagasins = await _contexte.Magasins
                .Include(m => m.Ventes)
                    .ThenInclude(v => v.Lignes)
                        .ThenInclude(l => l.Produit)
                .Include(m => m.StocksProduits)
                    .ThenInclude(sp => sp.Produit)
                .ToListAsync();
            _logger.LogInformation("{Count} magasins chargés.", listeMagasins.Count);

            var rapports = new List<RapportDto>();

            foreach (var magasin in listeMagasins)
            {
                _logger.LogInformation("Traitement du magasin : {NomMagasin}", magasin.Nom);

                decimal chiffreAffaires = magasin.Ventes
                    .SelectMany(v => v.Lignes)
                    .Sum(l => l.Quantite * l.PrixUnitaire);
                _logger.LogInformation("Chiffre d'affaires pour {NomMagasin} : {ChiffreAffaires:C}", magasin.Nom, chiffreAffaires);

                var topProduits = magasin.Ventes
                    .SelectMany(v => v.Lignes)
                    .GroupBy(l => l.Produit)
                    .Select(g => new InfosVenteProduit
                    {
                        NomProduit = g.Key.Nom,
                        QuantiteVendue = g.Sum(x => x.Quantite),
                        TotalVentes = g.Sum(x => x.Quantite * x.PrixUnitaire)
                    })
                    .OrderByDescending(info => info.QuantiteVendue)
                    .Take(3)
                    .ToList();

                _logger.LogInformation("Top 3 produits pour {NomMagasin} : {Produits}", magasin.Nom,
                    string.Join(", ", topProduits.Select(p => $"{p.NomProduit} ({p.QuantiteVendue})")));

                var stocksRestants = magasin.StocksProduits
                    .Select(sp => new InfosStockProduit
                    {
                        NomProduit = sp.Produit.Nom,
                        QuantiteRestante = sp.Quantite
                    })
                    .ToList();

                rapports.Add(new RapportDto
                {
                    NomMagasin = magasin.Nom,
                    ChiffreAffairesTotal = chiffreAffaires,
                    TopProduits = topProduits,
                    StocksRestants = stocksRestants
                });
            }

            _logger.LogInformation("Chargement du stock central...");
            var listeStockCentral = await _contexte.StocksCentraux
                .Include(sc => sc.Produit)
                .ToListAsync();
            _logger.LogInformation("{Count} entrées de stock central chargées.", listeStockCentral.Count);

            rapports.Add(new RapportDto
            {
                NomMagasin = "Stock Central",
                ChiffreAffairesTotal = 0m,
                TopProduits = new List<InfosVenteProduit>(),
                StocksRestants = listeStockCentral
                    .Select(sc => new InfosStockProduit
                    {
                        NomProduit = sc.Produit.Nom,
                        QuantiteRestante = sc.Quantite
                    })
                    .ToList()
            });

            _logger.LogInformation("Rapport consolidé généré avec succès.");
            return rapports;
        }
    }
}