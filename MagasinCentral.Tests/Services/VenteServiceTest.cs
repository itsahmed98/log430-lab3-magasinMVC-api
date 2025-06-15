using MagasinCentral.Data;
using MagasinCentral.Models;
using MagasinCentral.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MagasinCentral.Tests.Services
{
    public class VenteServiceTest
    {
        private async Task<MagasinDbContext> CreateInMemoryContextAsync()
        {
            var options = new DbContextOptionsBuilder<MagasinDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
                .Options;

            var context = new MagasinDbContext(options);
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync(); // Appelle la seed automatiquement

            return context;
        }

        private VenteService CreateService(MagasinDbContext context)
        {
            var loggerMock = new Mock<ILogger<VenteService>>();
            return new VenteService(context, loggerMock.Object);
        }

        [Fact]
        public void Constructeur_NullDbContext_ThrowsArgumentNullException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<VenteService>>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new VenteService(null!, loggerMock.Object));
        }

        [Fact]
        public async Task CreerVenteAsync_ValidData_CreatesVente()
        {
            // Arrange
            var magasinId = 1;
            var lignes = new List<(int produitId, int quantite)>
            {
                (3, 2),
                (2, 3)
            };

            var context = await CreateInMemoryContextAsync();
            var service = CreateService(context);

            // Act
            var venteId = await service.CreerVenteAsync(magasinId, lignes);

            // Assert
            Assert.True(venteId > 0);

            var vente = await context.Ventes
                .Include(v => v.Lignes)
                .FirstOrDefaultAsync(v => v.VenteId == venteId);

            Assert.NotNull(vente);
            Assert.Equal(2, vente.Lignes.Count);
        }
    }
}
