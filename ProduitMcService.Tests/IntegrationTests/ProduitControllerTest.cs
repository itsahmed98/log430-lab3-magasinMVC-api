using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using ProduitMcService.Controllers;
using ProduitMcService.Models;
using ProduitMcService.Services;
using System.Net;
using System.Net.Http.Json;


namespace ProduitMcService.Test.UnitTests.Controllers
{
    /// <summary>
    /// Tests integration pour le contrôleur API ProduitController.
    /// </summary>
    public class ProduitControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ProduitControllerTest(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenHttpClientFactoryIsNull()
        {
            var ILoggerMock = new Mock<ILogger<ProduitController>>();
            var IProduitServiceMock = new Mock<IProduitService>();

            Assert.Throws<ArgumentNullException>(() => new ProduitController(null!, IProduitServiceMock.Object));
            Assert.Throws<ArgumentNullException>(() => new ProduitController(ILoggerMock.Object, null!));
        }

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/v1/produits");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PostProduit_ReturnsCreated()
        {
            var produit = new Produit
            {
                Nom = "API Stylo",
                Prix = 3.25m
            };

            var response = await _client.PostAsJsonAsync("/api/v1/produits", produit);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task GetProduitById_ReturnsOkOrNotFound()
        {
            var response = await _client.GetAsync("/api/v1/produits/1");
            Assert.True(response.StatusCode is HttpStatusCode.OK or HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteProduit_ReturnsExpected()
        {
            var postResponse = await _client.PostAsJsonAsync("/api/v1/produits", new Produit
            {
                Nom = "ToDelete",
                Prix = 5
            });

            var created = await postResponse.Content.ReadFromJsonAsync<Produit>();
            var deleteResponse = await _client.DeleteAsync($"/api/v1/produits/{created!.ProduitId}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }
    }
}
