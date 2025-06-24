using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;
using PanierMcService.Data;
using PanierMcService.Models;
using PanierMcService.Services;

namespace PanierMcService.Test.UnitTest {
    public class PanierServiceTest
    {
        private readonly PanierDbContext _context;
        private readonly PanierService _service;
        private readonly Mock<ILogger<PanierService>> _loggerMock;
        private readonly Mock<IHttpClientFactory> _httpFactoryMock;
        private readonly HttpClient _httpClient;

        public PanierServiceTest()
        {
            var options = new DbContextOptionsBuilder<PanierDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new PanierDbContext(options);
            _loggerMock = new Mock<ILogger<PanierService>>();
            _httpClient = new HttpClient(new FakeHttpHandler()); // handler personnalisé
            _httpFactoryMock = new Mock<IHttpClientFactory>();
            _httpFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_httpClient);

            _service = new PanierService(_loggerMock.Object, _context, _httpFactoryMock.Object);
        }

        [Fact]
        public async Task AjouterOuMettreAJourProduit_ShouldAddProduct_WhenNotExists()
        {
            await _service.AjouterOuMettreAJourProduit(1, 100, 5);

            var panier = await _context.Paniers.Include(p => p.Lignes).FirstOrDefaultAsync();
            Assert.NotNull(panier);
            Assert.Single(panier!.Lignes);
            Assert.Equal(100, panier.Lignes[0].ProduitId);
            Assert.Equal(5, panier.Lignes[0].Quantite);
        }

        [Fact]
        public async Task SupprimerProduit_ShouldRemoveProduct_WhenExists()
        {
            await _service.AjouterOuMettreAJourProduit(1, 200, 3);
            await _service.SupprimerProduit(1, 200);

            var panier = await _context.Paniers.Include(p => p.Lignes).FirstOrDefaultAsync(p => p.ClientId == 1);
            Assert.Empty(panier!.Lignes);
        }

        [Fact]
        public async Task ViderPanier_ShouldClearAllLines()
        {
            await _service.AjouterOuMettreAJourProduit(1, 101, 2);
            await _service.AjouterOuMettreAJourProduit(1, 102, 4);
            await _service.ViderPanier(1);

            var panier = await _context.Paniers.Include(p => p.Lignes).FirstAsync();
            Assert.Empty(panier.Lignes);
        }

        [Fact]
        public async Task CalculerTotalAsync_ShouldReturnCorrectTotal()
        {
            // Arrange
            var handler = new FakeHttpHandler();
            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost:7198/api/v1/produits/")
            };

            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(f => f.CreateClient("ProduitMcService")).Returns(httpClient);

            var loggerMock = new Mock<ILogger<PanierService>>();
            var dbContext = new PanierDbContext(new DbContextOptionsBuilder<PanierDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

            var service = new PanierService(loggerMock.Object, dbContext, httpClientFactoryMock.Object);

            // Seed panier
            await service.AjouterOuMettreAJourProduit(1, 10, 2);
            var panier = await dbContext.Paniers.Include(p => p.Lignes).FirstAsync();

            // Act
            var total = await service.CalculerTotalAsync(panier.PanierId);

            // Assert
            Assert.Equal(20.0m, total);
        }

        private class FakeHttpHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var path = request.RequestUri?.AbsolutePath;

                if (path != null && path.StartsWith("/api/v1/produits/"))
                {
                    var produitId = int.Parse(path.Split('/').Last());
                    var produit = new ProduitDto { ProduitId = produitId, Prix = 10.0m };

                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = JsonContent.Create(produit)
                    });
                }

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
            }
        }
    }
}
