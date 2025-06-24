using System.Net;
using System.Net.Http.Json;
using VenteMcService.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using VenteMcService.Controllers;
using VenteMcService.Services;
using Moq;
using Microsoft.Extensions.Logging;

/// <summary>
/// Tests d'intégration pour le contrôleur de vente.
/// </summary>
public class VenteControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public VenteControllerTest(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenDependenciesAreNull()
    {
        var ILoggerMock = new Mock<ILogger<VenteController>>();
        var IVenteServiceMock = new Mock<IVenteService>();

        Assert.Throws<ArgumentNullException>(() => new VenteController(null!, IVenteServiceMock.Object));
        Assert.Throws<ArgumentNullException>(() => new VenteController(ILoggerMock.Object, null!));
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/v1/ventes");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    //[Fact]
    //public async Task PostVente_ReturnsCreated()
    //{
    //    var vente = new Vente
    //    {
    //        MagasinId = 1,
    //        Date = DateTime.UtcNow,
    //        Lignes = new List<LigneVente>
    //        {
    //            new LigneVente { ProduitId = 1, Quantite = 1, PrixUnitaire = 9.99m }
    //        }
    //    };

    //    var response = await _client.PostAsJsonAsync("/api/v1/ventes", vente);
    //    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    //}

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenInvalidId()
    {
        var response = await _client.GetAsync("/api/v1/ventes/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenIdDoesNotExist()
    {
        var response = await _client.DeleteAsync("/api/v1/ventes/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
