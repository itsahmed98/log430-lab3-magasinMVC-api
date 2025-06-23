using Microsoft.EntityFrameworkCore;
using StockMcService.Data;
using StockMcService.Models;

namespace StockMcService.Services
{
    /// <summary>
    /// Service pour la gestion des inventaires et des transferts de stock.
    /// </summary>
    public class StockService : IStockService
    {
        private readonly StockDbContext _contexte;
        public StockService(StockDbContext db) => _contexte = db;

        /// <inheritdoc/> 
        public async Task<IEnumerable<StockDto>> GetAllStocksAsync()
        {
            return await _contexte.StockItems
                .AsNoTracking()
                .Select(si => new StockDto
                {
                    MagasinId = si.MagasinId,
                    ProduitId = si.ProduitId,
                    Quantite = si.Quantite
                })
                .ToListAsync();
        }

        /// <inheritdoc/> 
        public async Task<StockDto?> GetStockByMagasinProduitAsync(int magasinId, int produitId)
        {
            var si = await _contexte.StockItems
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MagasinId == magasinId && x.ProduitId == produitId);
            if (si == null) return null;
            return new StockDto
            {
                MagasinId = si.MagasinId,
                ProduitId = si.ProduitId,
                Quantite = si.Quantite
            };
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<StockDto>> GetStockByMagasinAsync(int magasinId)
        {
            return await _contexte.StockItems
                .Where(s => s.MagasinId == magasinId)
                .Select(s => new StockDto
                {
                    ProduitId = s.ProduitId,
                    MagasinId = s.MagasinId,
                    Quantite = s.Quantite
                })
                .ToListAsync();
        }
    }
}
