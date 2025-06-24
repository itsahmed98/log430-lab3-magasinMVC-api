using Moq;
using RapportMcService.Models;
using System.Net;
using System.Net.Http.Json;

/// <summary>
/// Les tests d'intégration pour le contrôleur de rapport consolidé des ventes.
/// </summary>
public class RapportControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public RapportControllerIntegrationTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetRapport_ShouldReturn200_WhenServiceReturnsValidRapport()
    {
        // Arrange
        var rapport = new RapportVentesDto
        {
            DateGeneration = DateTime.UtcNow,
            VentesParMagasin = new(),
            ProduitsLesPlusVendus = new(),
            StocksRestants = new()
        };

        _factory.MockRapportService
            .Setup(s => s.ObtenirRapportConsolideAsync())
            .ReturnsAsync(rapport);

        // Act
        var response = await _client.GetAsync("/api/v1/rapports");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<RapportVentesDto>();
        Assert.NotNull(result);
        Assert.Equal(rapport.DateGeneration, result!.DateGeneration);
    }

    [Fact]
    public async Task GetRapport_ShouldReturn404_WhenServiceReturnsNull()
    {
        // Arrange
        _factory.MockRapportService
            .Setup(s => s.ObtenirRapportConsolideAsync())
            .ReturnsAsync((RapportVentesDto?)null);

        // Act
        var response = await _client.GetAsync("/api/v1/rapports");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Aucun rapport disponible", body);
    }
}
