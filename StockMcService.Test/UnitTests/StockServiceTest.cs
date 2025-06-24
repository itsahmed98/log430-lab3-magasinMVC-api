using Xunit;
using StockMcService.Services;
using StockMcService.Models;
using StockMcService.Data;
using Microsoft.EntityFrameworkCore;
using Castle.Core.Logging;
using Moq;
using Microsoft.Extensions.Logging;

namespace StockMcService.Test.UnitTests
{
    public class StockServiceTest
    {
        private readonly DbContextOptions<StockDbContext> _options;
        private readonly Mock<ILogger<StockService>> _loggerMock = new Mock<ILogger<StockService>>();

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

            var service = new StockService(context, _loggerMock.Object);
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

            var service = new StockService(context, _loggerMock.Object);
            var stock = await service.GetStockByMagasinProduitAsync(1, 2);

            Assert.NotNull(stock);
            Assert.Equal(30, stock!.Quantite);
        }

        [Fact]
        public async Task GetStockByMagasinProduitAsync_ShouldReturnNull_WhenNotFound()
        {
            await using var context = new StockDbContext(_options);
            var service = new StockService(context, _loggerMock.Object);

            var stock = await service.GetStockByMagasinProduitAsync(999, 999);
            Assert.Null(stock);
        }
    }
}