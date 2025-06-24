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
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock2;
    private readonly VenteService _service;

    public VenteServiceTest()
    {
        var options = new DbContextOptionsBuilder<VenteDbContext>()
            .UseInMemoryDatabase(databaseName: "VenteServiceTest")
            .Options;
        _context = new VenteDbContext(options);

        _loggerMock = new Mock<ILogger<VenteService>>();
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _httpClientFactoryMock2 = new Mock<IHttpClientFactory>();

        var produit = new ProduitDto { ProduitId = 1, Nom = "Test", Prix = 10.0m };
        var client = new HttpClient(new MockHttpMessageHandler(JsonSerializer.Serialize(produit)))
        {
            BaseAddress = new Uri("http://localhost")
        };
        _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);
        _httpClientFactoryMock2.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

        _service = new VenteService(_loggerMock.Object, _context, _httpClientFactoryMock.Object, _httpClientFactoryMock2.Object);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenContextIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new VenteService(_loggerMock.Object, null!, _httpClientFactoryMock.Object, _httpClientFactoryMock2.Object));
        Assert.Throws<ArgumentNullException>(() => new VenteService(null!, _context, _httpClientFactoryMock.Object, _httpClientFactoryMock2.Object));
        Assert.Throws<ArgumentNullException>(() => new VenteService(_loggerMock.Object, _context, null!, _httpClientFactoryMock2.Object));
        Assert.Throws<ArgumentNullException>(() => new VenteService(_loggerMock.Object, _context, _httpClientFactoryMock.Object, null!));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAll()
    {
        var ventes = await _service.GetAllAsync();
        Assert.NotNull(ventes);
    }

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