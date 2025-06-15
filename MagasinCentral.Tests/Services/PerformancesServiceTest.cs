using MagasinCentral.Data;
using MagasinCentral.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace MagasinCentral.Tests.Services
{
    /// <summary>
    /// Tests unitaires pour le service PerformancesService.
    /// </summary>
    public class PerformancesServiceTest
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

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenContextIsNull()
        {
            // Arrange
            MagasinDbContext? context = null;
            var loggerMock = new Mock<ILogger<PerformancesService>>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new PerformancesService(loggerMock.Object, context!));
        }

        [Fact]
        public async Task GetPerformances_ShouldReturnPerformances()
        {
            // Arrange
            var context = await CreateInMemoryContextAsync();
            var loggerMock = new Mock<ILogger<PerformancesService>>();
            var service = new PerformancesService(loggerMock.Object, context);

            // Act
            var result = await service.GetPerformances();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.RevenusParMagasin.Count); // Il y a 4 magasins dans le DataSeeder
            Assert.Contains(result.RevenusParMagasin, r => r.ChiffreAffaires > 0);
            Assert.Equal(1, result.ProduitsRupture.Count); // Stylo en rupture pour MagasinId = 1
            Assert.True(result.TendancesHebdomadairesParMagasin.Count >= 1);
            Assert.All(result.TendancesHebdomadairesParMagasin.Values, liste => Assert.Equal(7, liste.Count));
        }
    }
}
