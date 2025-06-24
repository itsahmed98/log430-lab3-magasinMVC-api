using Microsoft.Extensions.Logging;
using ProduitMcService.Models;
using ProduitMcService.Services;
using ProduitMcService.Data;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ProduitMcService.Test.UnitTests.Services
{
    /// <summary>
    /// Tests unitaires pour le service ProduitService.
    /// </summary>
    public class ProduitServiceTest
    {
        private readonly ProduitDbContext _context;
        private readonly ProduitService _service;

        public ProduitServiceTest()
        {
            var options = new DbContextOptionsBuilder<ProduitDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ProduitDbContext(options);

            var loggerMock = new Mock<ILogger<ProduitService>>();
            _service = new ProduitService(loggerMock.Object, _context);
        }

        [Fact]
        public void Constructeur_ShouldThrowArgumentNullException()
        {
            var loggerMock = new Mock<ILogger<ProduitService>>();
            Assert.Throws<ArgumentNullException>(() => new ProduitService(loggerMock.Object, null!));
            Assert.Throws<ArgumentNullException>(() => new ProduitService(null!, _context));
        }

        [Fact]
        public async Task CreateAsync_ShouldAddProduit()
        {
            var produit = new Produit { Nom = "Test", Prix = 10 };
            var result = await _service.CreateAsync(produit);

            Assert.NotNull(result);
            Assert.Equal("Test", result.Nom);
            Assert.Single(await _context.Produits.ToListAsync());
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProduits()
        {
            _context.Produits.Add(new Produit { Nom = "Produit1", Prix = 5 });
            _context.Produits.Add(new Produit { Nom = "Produit2", Prix = 15 });
            await _context.SaveChangesAsync();

            var list = await _service.GetAllAsync();
            Assert.Equal(2, list.Count);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnProduit()
        {
            var produit = new Produit { Nom = "ProduitX", Prix = 9 };
            _context.Produits.Add(produit);
            await _context.SaveChangesAsync();

            var result = await _service.GetByIdAsync(produit.ProduitId);
            Assert.NotNull(result);
            Assert.Equal("ProduitX", result.Nom);
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistingProduit_ShouldThrow()
        {
            var produit = new Produit { ProduitId = 999, Nom = "X", Prix = 1 };
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(produit));
            Assert.Contains("introuvable", ex.Message);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveProduit()
        {
            var produit = new Produit { Nom = "ToDelete", Prix = 5 };
            _context.Produits.Add(produit);
            await _context.SaveChangesAsync();

            await _service.DeleteAsync(produit.ProduitId);
            var deleted = await _context.Produits.FindAsync(produit.ProduitId);
            Assert.Null(deleted);
        }
    }
}