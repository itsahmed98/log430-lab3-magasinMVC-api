using System;
using System.Threading.Tasks;
using MagasinCentral.Data;
using MagasinCentral.Models;
using MagasinCentral.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace MagasinCentral.Tests.Services
{
    /// <summary>
    /// Tests unitaires pour le service ProduitService.
    /// </summary>
    public class ProduitServiceTest
    {
        /// <summary>
        /// Crée un contexte InMemory unique pour chaque test.
        /// </summary>
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

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ProduitService(cache!, context));
        }

        [Fact]
        public async Task Constructor_ShouldThrowArgumentNullException_WhenContextIsNull()
        {
            // Arrange
            var cache = new MemoryCache(new MemoryCacheOptions());
            MagasinDbContext? context = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ProduitService(cache, context!));
        }

        [Fact]
        public async Task GetAllProduitsAsync_ShouldReturnNonNullList()
        {
            // Arrange
            var context = await CreateInMemoryContextAsync();
            var cache = new MemoryCache(new MemoryCacheOptions());
            var service = new ProduitService(cache, context);

            // Act
            var produits = await service.GetAllProduitsAsync();

            // Assert
            Assert.NotNull(produits);
        }

        [Fact]
        public async Task GetProduitByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            var context = await CreateInMemoryContextAsync();
            var cache = new MemoryCache(new MemoryCacheOptions());
            var service = new ProduitService(cache, context);

            // Act
            var result = await service.GetProduitByIdAsync(99999); // ID très peu probable d'exister

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetProduitByIdAsync_ShouldReturnProduit_WhenExists()
        {
            // Arrange
            var context = await CreateInMemoryContextAsync();
            var cache = new MemoryCache(new MemoryCacheOptions());
            var service = new ProduitService(cache, context);

            // Pour tester un produit existant, soit votre seeder a inséré un produit avec un ID connu (par exemple 1),
            // soit on ajoute manuellement un produit avec un ID unique:
            var nouveauProduit = new Produit { ProduitId = 200, Nom = "ExistantTest", Prix = 5.50m };
            context.Produits.Add(nouveauProduit);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetProduitByIdAsync(200);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.ProduitId);
            Assert.Equal("ExistantTest", result.Nom);
        }

        [Fact]
        public async Task ModifierProduitAsync_ShouldUpdateAndInvalidateCache()
        {
            // Arrange
            var context = await CreateInMemoryContextAsync();
            var cache = new MemoryCache(new MemoryCacheOptions());
            var service = new ProduitService(cache, context);

            // Ajouter un produit pour modification
            var produit = new Produit { ProduitId = 300, Nom = "Original", Prix = 1.00m };
            context.Produits.Add(produit);
            await context.SaveChangesAsync();

            // Charger dans le cache
            var cached = await service.GetProduitByIdAsync(300);
            Assert.NotNull(cached);
            // Modifier l'objet
            produit.Nom = "Modifié";
            produit.Prix = 2.00m;

            // Act
            await service.ModifierProduitAsync(produit);

            // Après modification, on attend à la fois dans la BDD et que le cache ait été invalidé.
            var produitMisAJour = await context.Produits.AsNoTracking().FirstOrDefaultAsync(p => p.ProduitId == 300);
            Assert.Equal("Modifié", produitMisAJour.Nom);
            Assert.Equal(2.00m, produitMisAJour.Prix);

            // Le cache ayant été invalidé, un nouvel appel devrait refléter la version modifiée
            var resultCacheApres = await service.GetProduitByIdAsync(300);
            Assert.NotNull(resultCacheApres);
            Assert.Equal("Modifié", resultCacheApres.Nom);
        }

        [Fact]
        public async Task RechercherProduitsAsync_ShouldReturnMatchingResults()
        {
            // Arrange
            var context = await CreateInMemoryContextAsync();
            var cache = new MemoryCache(new MemoryCacheOptions());
            var service = new ProduitService(cache, context);

            // Ajouter plusieurs produits pour la recherche
            context.Produits.Add(new Produit { ProduitId = 400, Nom = "StyloBleu", Categorie = "Papeterie", Prix = 1.00m });
            context.Produits.Add(new Produit { ProduitId = 401, Nom = "Ordinateur", Categorie = "Électronique", Prix = 1000.00m });
            await context.SaveChangesAsync();

            // Act
            var resultNom = await service.RechercherProduitsAsync("stylo");
            var resultCategorie = await service.RechercherProduitsAsync("électronique");
            var resultIdString = await service.RechercherProduitsAsync("401");

            // Assert
            Assert.Contains(resultNom, p => p.ProduitId == 400);
            Assert.Contains(resultCategorie, p => p.ProduitId == 401);
            Assert.Contains(resultIdString, p => p.ProduitId == 401);
        }
    }
}
