using MagasinCentral.Data;
using MagasinCentral.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Memory;

namespace MagasinCentral.Services
{
    public class StockService : IStockService
    {
        private readonly MagasinDbContext _contexte;
        private readonly IMemoryCache _cache;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

        public StockService(MagasinDbContext contexte, IMemoryCache cache)
        {
            _contexte = contexte ?? throw new ArgumentNullException(nameof(contexte));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        /// <inheritdoc />
        public async Task<int?> GetStockByMagasinId(int magasinId)
        {
            var magasin = await _contexte.Magasins
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MagasinId == magasinId);

            if (magasin == null)
                return null;

            string key = $"stock_{magasinId}";
            return await _cache.GetOrCreateAsync(key, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheDuration;
                return _contexte.MagasinStocksProduits
                    .Where(ms => ms.MagasinId == magasinId)
                    .Select(ms => (int?)ms.Quantite)
                    .FirstOrDefaultAsync();
            });
        }
    }
}