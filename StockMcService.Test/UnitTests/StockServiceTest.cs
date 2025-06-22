using Xunit;
using StockMcService.Services;
using StockMcService.Models;
using StockMcService.Data;
using Microsoft.EntityFrameworkCore;

namespace StockMcService.Test.UnitTests
{
    public class StockServiceTest
    {
        private readonly DbContextOptions<StockDbContext> _options;

        public StockServiceTest()
        {
            _options = new DbContextOptionsBuilder<StockDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GetAllStocksAsync_ShouldReturnAllStocks()
        {
            await using var context = new StockDbContext(_options);
            context.StockItems.Add(new StockItem { MagasinId = 1, ProduitId = 2, Quantite = 50 });
            await context.SaveChangesAsync();

            var service = new StockService(context);
            var stocks = await service.GetAllStocksAsync();

            Assert.Single(stocks);
            Assert.Equal(50, stocks.First().Quantite);
        }

        [Fact]
        public async Task GetStockByMagasinProduitAsync_ShouldReturnStock_WhenFound()
        {
            await using var context = new StockDbContext(_options);
            context.StockItems.Add(new StockItem { MagasinId = 1, ProduitId = 2, Quantite = 30 });
            await context.SaveChangesAsync();

            var service = new StockService(context);
            var stock = await service.GetStockByMagasinProduitAsync(1, 2);

            Assert.NotNull(stock);
            Assert.Equal(30, stock!.Quantite);
        }

        [Fact]
        public async Task GetStockByMagasinProduitAsync_ShouldReturnNull_WhenNotFound()
        {
            await using var context = new StockDbContext(_options);
            var service = new StockService(context);

            var stock = await service.GetStockByMagasinProduitAsync(999, 999);
            Assert.Null(stock);
        }
    }
}