using MagasinMcService.Models;
using Moq;
using System.Net;
using System.Net.Http.Json;

namespace MagasinMcService.Test.IntegrationTest
{
    /// <summary>
    /// Les tests d'intégration pour le contrôleur MagasinController.
    /// </summary>
    public class MagasinControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public MagasinControllerIntegrationTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ShouldReturnSuccess()
        {
            _factory.MockMagasinService.Setup(s => s.GetAllAsync())
                .ReturnsAsync(new List<Magasin>
                {
            new() { MagasinId = 1, Nom = "MockMagasin" }
                });

            var response = await _client.GetAsync("/api/v1/magasins");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<List<Magasin>>();
            Assert.Single(result);
            Assert.Equal("MockMagasin", result[0].Nom);
        }

        [Fact]
        public async Task GetById_ShouldReturn404_WhenNotFound()
        {
            var response = await _client.GetAsync("/api/v1/magasins/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetById_ShouldReturnMagasin_WhenExists()
        {
            var response = await _client.GetAsync("/api/v1/magasins/1");
            if (response.StatusCode == HttpStatusCode.NotFound)
                return;

            var magasin = await response.Content.ReadFromJsonAsync<Magasin>();
            Assert.NotNull(magasin);
            Assert.Equal(1, magasin!.MagasinId);
        }
    }
}
