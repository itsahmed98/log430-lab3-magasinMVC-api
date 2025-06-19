using Microsoft.EntityFrameworkCore;
using StockMcService.Data;
using StockMcService.Models;

namespace StockMcService.Services
{
    public class StockService : IStockService
    {
        private readonly StockDbContext _contexte;
        public StockService(StockDbContext db) => _contexte = db;

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
    }
}
