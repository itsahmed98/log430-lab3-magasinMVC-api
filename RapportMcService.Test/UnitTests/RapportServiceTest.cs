using Moq;
using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using RapportMcService.Services;
using RapportMcService.Models;
using Moq.Protected;

/// <summary>
/// Les tests unitaires pour le service de rapport consolidé des ventes.
/// </summary>
public class RapportServiceTest
{
    private Mock<ILogger<RapportService>> _mockLogger = new();
    private Mock<IHttpClientFactory> _mockFactoryVente = new();
    private Mock<IHttpClientFactory> _mockFactoryProduit = new();
    private Mock<IHttpClientFactory> _mockFactoryStock = new();

    private HttpClient CreateMockHttpClient<T>(T responseData)
    {
        var messageHandler = new Mock<HttpMessageHandler>();
        messageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = responseData != null ? JsonContent.Create(responseData) : new StringContent("null")
            });

        var client = new HttpClient(messageHandler.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };

        return client;
    }

    [Fact]
    public void Constructeur_WithNullLogger_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RapportService(null!, _mockFactoryVente.Object, _mockFactoryProduit.Object, _mockFactoryStock.Object));
    }

    [Fact]
    public async Task ObtenirRapportConsolideAsync_ShouldReturnRapport_WhenAllMicroservicesRespondCorrectly()
    {
        // Arrange
        var ventes = new List<VenteDto>
        {
            new()
            {
                MagasinId = 1,
                Lignes = new List<LigneVenteDto>
                {
                    new() { ProduitId = 1, Quantite = 2, PrixUnitaire = 10 },
                    new() { ProduitId = 2, Quantite = 1, PrixUnitaire = 5 }
                }
            }
        };

        var produits = new List<ProduitDto>
        {
            new() { ProduitId = 1, Nom = "Stylo" },
            new() { ProduitId = 2, Nom = "Carnet" }
        };

        var stocks = new List<StockDto>
        {
            new() { MagasinId = 1, ProduitId = 1, Quantite = 100 },
            new() { MagasinId = 1, ProduitId = 2, Quantite = 50 }
        };

        var clientVente = CreateMockHttpClient(ventes);
        var clientProduit = CreateMockHttpClient(produits);
        var clientStock = CreateMockHttpClient(stocks);

        _mockFactoryVente.Setup(f => f.CreateClient("VenteMcService")).Returns(clientVente);
        _mockFactoryProduit.Setup(f => f.CreateClient("ProduitMcService")).Returns(clientProduit);
        _mockFactoryStock.Setup(f => f.CreateClient("StockMcService")).Returns(clientStock);

        var service = new RapportService(_mockLogger.Object,
                                         _mockFactoryVente.Object,
                                         _mockFactoryProduit.Object,
                                         _mockFactoryStock.Object);

        // Act
        var result = await service.ObtenirRapportConsolideAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.VentesParMagasin.Count);
        Assert.Equal(2, result.ProduitsLesPlusVendus.Count);
        Assert.Equal(2, result.StocksRestants.Count);
    }

    [Fact]
    public async Task ObtenirRapportConsolideAsync_WhenMicroserviceReturnsNull_ShouldNotThrow()
    {
        // Arrange
        var clientVente = CreateMockHttpClient<object>(null);
        var clientProduit = CreateMockHttpClient<object>(null);
        var clientStock = CreateMockHttpClient<object>(null);

        _mockFactoryVente.Setup(f => f.CreateClient("VenteMcService")).Returns(clientVente);
        _mockFactoryProduit.Setup(f => f.CreateClient("ProduitMcService")).Returns(clientProduit);
        _mockFactoryStock.Setup(f => f.CreateClient("StockMcService")).Returns(clientStock);

        var service = new RapportService(_mockLogger.Object,
                                         _mockFactoryVente.Object,
                                         _mockFactoryProduit.Object,
                                         _mockFactoryStock.Object);

        // Act
        var result = await service.ObtenirRapportConsolideAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.VentesParMagasin);
        Assert.Empty(result.ProduitsLesPlusVendus);
        Assert.Empty(result.StocksRestants);
    }

    [Fact]
    public async Task ObtenirRapportConsolideAsync_WhenHttpRequestFails_ShouldThrowApplicationException()
    {
        // Arrange
        var messageHandler = new Mock<HttpMessageHandler>();
        messageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Service non disponible"));

        var client = new HttpClient(messageHandler.Object);

        _mockFactoryVente.Setup(f => f.CreateClient("VenteMcService")).Returns(client);
        _mockFactoryProduit.Setup(f => f.CreateClient("ProduitMcService")).Returns(client);
        _mockFactoryStock.Setup(f => f.CreateClient("StockMcService")).Returns(client);

        var service = new RapportService(_mockLogger.Object,
                                         _mockFactoryVente.Object,
                                         _mockFactoryProduit.Object,
                                         _mockFactoryStock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ApplicationException>(() => service.ObtenirRapportConsolideAsync());
    }
}
