using MagasinMcService.Data;
using MagasinMcService.Models;
using MagasinMcService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace MagasinMcService.Test.UnitTest
{
    /// <summary>
    /// Les tests unitaires pour le service MagasinService.
    /// </summary>
    public class MagasinServiceTests
    {
        private readonly DbContextOptions<MagasinDbContext> _options;

        public MagasinServiceTests()
        {
            _options = new DbContextOptionsBuilder<MagasinDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task Constructor_ShouldThrow_WhenDependenciesAreNull()
        {
            await using var context = new MagasinDbContext(_options);
            var logger = new Mock<ILogger<MagasinService>>();
            Assert.Throws<ArgumentNullException>(() =>
                new MagasinService(null!, context));
            Assert.Throws<ArgumentNullException>(() =>
                new MagasinService(logger.Object, null!));
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllMagasins()
        {
            // Arrange
            await using var context = new MagasinDbContext(_options);
            context.Magasins.AddRange(
                new Magasin { MagasinId = 1, Nom = "Magasin A" },
                new Magasin { MagasinId = 2, Nom = "Magasin B" });
            await context.SaveChangesAsync();

            var logger = new Mock<ILogger<MagasinService>>();
            var service = new MagasinService(logger.Object, context);

            // Act
            var magasins = await service.GetAllAsync();

            // Assert
            Assert.Equal(2, magasins.Count);
        }

        [Fact]
        public async Task GetMagasinByIdAsync_ShouldReturnMagasin_WhenFound()
        {
            await using var context = new MagasinDbContext(_options);
            var magasin = new Magasin { MagasinId = 1, Nom = "Test" };
            context.Magasins.Add(magasin);
            await context.SaveChangesAsync();

            var logger = new Mock<ILogger<MagasinService>>();
            var service = new MagasinService(logger.Object, context);

            var result = await service.GetMagasinByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Test", result!.Nom);
        }

        [Fact]
        public async Task GetMagasinByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            await using var context = new MagasinDbContext(_options);
            var logger = new Mock<ILogger<MagasinService>>();
            var service = new MagasinService(logger.Object, context);

            var result = await service.GetMagasinByIdAsync(999);

            Assert.Null(result);
        }
    }
}
