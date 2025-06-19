using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using VenteMcService.Data;
using VenteMcService.Models;
using VenteMcService.Services;
using Xunit;

/// <summary>
/// Tests unitaires pour le service de vente.
/// </summary>
public class VenteServiceTest
{
    private readonly VenteDbContext _context;
    private readonly Mock<ILogger<VenteService>> _loggerMock;
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly VenteService _service;

    public VenteServiceTest()
    {
        var options = new DbContextOptionsBuilder<VenteDbContext>()
            .UseInMemoryDatabase(databaseName: "VenteServiceTest")
            .Options;
        _context = new VenteDbContext(options);

        _loggerMock = new Mock<ILogger<VenteService>>();
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();

        var produit = new ProduitDto { ProduitId = 1, Nom = "Test", Prix = 10.0m };
        var client = new HttpClient(new MockHttpMessageHandler(JsonSerializer.Serialize(produit)))
        {
            BaseAddress = new Uri("http://localhost")
        };
        _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

        _service = new VenteService(_loggerMock.Object, _context, _httpClientFactoryMock.Object);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenContextIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new VenteService(_loggerMock.Object, null!, _httpClientFactoryMock.Object));
        Assert.Throws<ArgumentNullException>(() => new VenteService(null!, _context, _httpClientFactoryMock.Object));
        Assert.Throws<ArgumentNullException>(() => new VenteService(_loggerMock.Object, _context, null!));
    }

    //[Fact]
    //public async Task CreateAsync_ShouldCreateVente()
    //{
    //    var vente = new Vente
    //    {
    //        MagasinId = 1,
    //        Date = DateTime.UtcNow,
    //        Lignes = new List<LigneVente>
    //        {
    //            new LigneVente { ProduitId = 1, Quantite = 2 }
    //        }
    //    };

    //    var created = await _service.CreateAsync(vente);
    //    Assert.NotEqual(0, created.VenteId);
    //    Assert.Single(_context.Ventes);
    //    Assert.Single(_context.LignesVente);
    //}

    [Fact]
    public async Task GetAllAsync_ShouldReturnAll()
    {
        var ventes = await _service.GetAllAsync();
        Assert.NotNull(ventes);
    }

    //[Fact]
    //public async Task GetByIdAsync_ShouldReturnVenteIfExists()
    //{
    //    var vente = await _service.CreateAsync(new Vente
    //    {
    //        MagasinId = 2,
    //        Date = DateTime.UtcNow,
    //        Lignes = new List<LigneVente> { new LigneVente { ProduitId = 1, Quantite = 1 } }
    //    });

    //    var result = await _service.GetByIdAsync(vente.VenteId);
    //    Assert.NotNull(result);
    //}

    //[Fact]
    //public async Task DeleteAsync_ShouldDeleteVente()
    //{
    //    var vente = await _service.CreateAsync(new Vente
    //    {
    //        MagasinId = 3,
    //        Date = DateTime.UtcNow,
    //        Lignes = new List<LigneVente> { new LigneVente { ProduitId = 1, Quantite = 1 } }
    //    });

    //    await _service.DeleteAsync(vente.VenteId);
    //    var result = await _service.GetByIdAsync(vente.VenteId);
    //    Assert.Null(result);
    //}

    private class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _json;

        public MockHttpMessageHandler(string json)
        {
            _json = json;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_json, Encoding.UTF8, "application/json")
            };
            return Task.FromResult(response);
        }
    }
}