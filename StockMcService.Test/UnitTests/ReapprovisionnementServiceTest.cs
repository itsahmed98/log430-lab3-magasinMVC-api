using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using StockMcService.Data;
using StockMcService.Models;
using StockMcService.Services;

namespace StockMcService.Test.UnitTests
{
    public class ReapprovisionnementServiceTest
    {
        private readonly DbContextOptions<StockDbContext> _options;

        public ReapprovisionnementServiceTest()
        {
            _options = new DbContextOptionsBuilder<StockDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task CréerDemandeAsync_ShouldAddDemand()
        {
            await using var context = new StockDbContext(_options);
            var logger = new Mock<ILogger<ReapprovisionnementService>>();
            var inventaire = new Mock<IInventaireService>();

            var service = new ReapprovisionnementService(logger.Object, context, inventaire.Object);

            var demande = await service.CréerDemandeAsync(1, 2, 10);

            Assert.NotNull(demande);
            Assert.Equal("EN_ATTENTE", demande.Statut);
        }

        [Fact]
        public async Task GetDemandesEnAttenteAsync_ShouldReturnOnlyPending()
        {
            await using var context = new StockDbContext(_options);
            context.Demandes.AddRange(
                new DemandeReapprovisionnement { MagasinId = 1, ProduitId = 2, Statut = "EN_ATTENTE", QuantiteDemandee = 5 },
                new DemandeReapprovisionnement { MagasinId = 2, ProduitId = 3, Statut = "VALIDÉE", QuantiteDemandee = 5 }
            );
            await context.SaveChangesAsync();

            var service = new ReapprovisionnementService(Mock.Of<ILogger<ReapprovisionnementService>>(), context, Mock.Of<IInventaireService>());
            var demandes = await service.GetDemandesEnAttenteAsync();

            Assert.Single(demandes);
            Assert.Equal("EN_ATTENTE", demandes.First().Statut);
        }

        [Fact]
        public async Task ValiderDemandeAsync_ShouldUpdateStatus_WhenTransferSucceeds()
        {
            await using var context = new StockDbContext(_options);
            context.Demandes.Add(new DemandeReapprovisionnement { DemandeId = 1, MagasinId = 1, ProduitId = 2, QuantiteDemandee = 10, Statut = "EN_ATTENTE" });
            await context.SaveChangesAsync();

            var inventaireMock = new Mock<IInventaireService>();
            inventaireMock.Setup(i => i.TransférerStockAsync(2, 1, 10)).ReturnsAsync(true);

            var service = new ReapprovisionnementService(Mock.Of<ILogger<ReapprovisionnementService>>(), context, inventaireMock.Object);
            var result = await service.ValiderDemandeAsync(1);

            Assert.True(result);
            Assert.Equal("VALIDÉE", context.Demandes.First().Statut);
        }
    }

}
