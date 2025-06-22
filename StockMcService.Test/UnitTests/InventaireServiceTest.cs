using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using StockMcService.Data;
using StockMcService.Models;
using StockMcService.Services;

namespace StockMcService.Test.UnitTests
{
    public class InventaireServiceTests
    {
        private readonly DbContextOptions<StockDbContext> _options;

        public InventaireServiceTests()
        {
            _options = new DbContextOptionsBuilder<StockDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task TransférerStockAsync_ShouldTransfer_WhenEnoughCentralStock()
        {
            await using var context = new StockDbContext(_options);
            context.StockItems.Add(new StockItem { MagasinId = 0, ProduitId = 1, Quantite = 100 });
            await context.SaveChangesAsync();

            var service = new InventaireService(context, Mock.Of<ILogger<InventaireService>>());
            var result = await service.TransférerStockAsync(1, 2, 20);

            Assert.True(result);

            var stockLocal = await context.StockItems.FirstOrDefaultAsync(s => s.MagasinId == 2 && s.ProduitId == 1);
            Assert.NotNull(stockLocal);
            Assert.Equal(20, stockLocal!.Quantite);
        }

        [Fact]
        public async Task TransférerStockAsync_ShouldFail_WhenNotEnoughStock()
        {
            await using var context = new StockDbContext(_options);
            context.StockItems.Add(new StockItem { MagasinId = 0, ProduitId = 1, Quantite = 5 });
            await context.SaveChangesAsync();

            var service = new InventaireService(context, Mock.Of<ILogger<InventaireService>>());
            var result = await service.TransférerStockAsync(1, 2, 10);

            Assert.False(result);
        }
    }
}
