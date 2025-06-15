using System;
using System.Threading.Tasks;
using MagasinCentral.Data;
using MagasinCentral.Models;
using MagasinCentral.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MagasinCentral.Tests.Services
{
    /// <summary>
    /// Tests unitaires pour le service ProduitService.
    /// </summary>
    public class ProduitServiceTest
    {
        private async Task<MagasinDbContext> CreateInMemoryContextAsync()
        {
            var options = new DbContextOptionsBuilder<MagasinDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
                .Options;

            var context = new MagasinDbContext(options);
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            return context;
        }

        [Fact]
        public async Task Constructor_ShouldThrowArgumentNullException_WhenCacheIsNull()
        {
            // Arrange
            IMemoryCache? cache = null;
            var context = await CreateInMemoryContextAsync();
            var loggerMock = new Mock<ILogger<ProduitService>>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ProduitService(cache!, context, loggerMock.Object));
        }

        [Fact]
        public async Task Constructor_ShouldThrowArgumentNullException_WhenContextIsNull()
        {
            // Arrange
            var cache = new MemoryCache(new MemoryCacheOptions());
            MagasinDbContext? context = null;
            var loggerMock = new Mock<ILogger<ProduitService>>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ProduitService(cache, context!, loggerMock.Object));
        }

        [Fact]
        public async Task GetAllProduitsAsync_ShouldReturnNonNullList()
        {
            var context = await CreateInMemoryContextAsync();
            var cache = new MemoryCache(new MemoryCacheOptions());
            var loggerMock = new Mock<ILogger<ProduitService>>();
            var service = new ProduitService(cache, context, loggerMock.Object);

            var produits = await service.GetAllProduitsAsync();
            Assert.NotNull(produits);
        }

        [Fact]
        public async Task GetProduitByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            var context = await CreateInMemoryContextAsync();
            var cache = new MemoryCache(new MemoryCacheOptions());
            var loggerMock = new Mock<ILogger<ProduitService>>();
            var service = new ProduitService(cache, context, loggerMock.Object);

            var result = await service.GetProduitByIdAsync(99999);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetProduitByIdAsync_ShouldReturnProduit_WhenExists()
        {
            var context = await CreateInMemoryContextAsync();
            var cache = new MemoryCache(new MemoryCacheOptions());
            var loggerMock = new Mock<ILogger<ProduitService>>();
            var service = new ProduitService(cache, context, loggerMock.Object);

            var nouveauProduit = new Produit { ProduitId = 200, Nom = "ExistantTest", Prix = 5.50m };
            context.Produits.Add(nouveauProduit);
            await context.SaveChangesAsync();

            var result = await service.GetProduitByIdAsync(200);
            Assert.NotNull(result);
            Assert.Equal(200, result.ProduitId);
            Assert.Equal("ExistantTest", result.Nom);
        }

        [Fact]
        public async Task ModifierProduitAsync_ShouldUpdateAndInvalidateCache()
        {
            var context = await CreateInMemoryContextAsync();
            var cache = new MemoryCache(new MemoryCacheOptions());
            var loggerMock = new Mock<ILogger<ProduitService>>();
            var service = new ProduitService(cache, context, loggerMock.Object);

            var produit = new Produit { ProduitId = 300, Nom = "Original", Prix = 1.00m };
            context.Produits.Add(produit);
            await context.SaveChangesAsync();

            var cached = await service.GetProduitByIdAsync(300);
            Assert.NotNull(cached);

            produit.Nom = "Modifié";
            produit.Prix = 2.00m;
            await service.ModifierProduitAsync(produit);

            var produitMisAJour = await context.Produits.AsNoTracking().FirstOrDefaultAsync(p => p.ProduitId == 300);
            Assert.Equal("Modifié", produitMisAJour.Nom);
            Assert.Equal(2.00m, produitMisAJour.Prix);

            var resultCacheApres = await service.GetProduitByIdAsync(300);
            Assert.NotNull(resultCacheApres);
            Assert.Equal("Modifié", resultCacheApres.Nom);
        }

        [Fact]
        public async Task RechercherProduitsAsync_ShouldReturnMatchingResults()
        {
            var context = await CreateInMemoryContextAsync();
            var cache = new MemoryCache(new MemoryCacheOptions());
            var loggerMock = new Mock<ILogger<ProduitService>>();
            var service = new ProduitService(cache, context, loggerMock.Object);

            context.Produits.Add(new Produit { ProduitId = 400, Nom = "StyloBleu", Categorie = "Papeterie", Prix = 1.00m });
            context.Produits.Add(new Produit { ProduitId = 401, Nom = "Ordinateur", Categorie = "Électronique", Prix = 1000.00m });
            await context.SaveChangesAsync();

            var resultNom = await service.RechercherProduitsAsync("stylo");
            var resultCategorie = await service.RechercherProduitsAsync("électronique");
            var resultIdString = await service.RechercherProduitsAsync("401");

            Assert.Contains(resultNom, p => p.ProduitId == 400);
            Assert.Contains(resultCategorie, p => p.ProduitId == 401);
            Assert.Contains(resultIdString, p => p.ProduitId == 401);
        }
    }
}
