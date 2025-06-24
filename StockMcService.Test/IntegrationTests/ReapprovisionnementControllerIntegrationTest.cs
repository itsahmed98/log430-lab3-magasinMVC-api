using Moq;
using StockMcService.Models;
using System.Net;
using System.Net.Http.Json;

namespace StockMcService.Test.IntegrationTests
{
    public class ReapprovisionnementControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public ReapprovisionnementControllerIntegrationTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Post_CréerDemande_ShouldReturnCreated()
        {
            var dto = new CreerDemandeDto
            {
                MagasinId = 1,
                ProduitId = 2,
                QuantiteDemandee = 10
            };

            // Configure le mock pour retourner une fausse demande
            _factory.MockReapprovisionnementService
                .Setup(s => s.CréerDemandeAsync(dto.MagasinId, dto.ProduitId, dto.QuantiteDemandee))
                .ReturnsAsync(new DemandeReapprovisionnement
                {
                    DemandeId = 123,
                    MagasinId = dto.MagasinId,
                    ProduitId = dto.ProduitId,
                    QuantiteDemandee = dto.QuantiteDemandee,
                    Statut = "EN_ATTENTE"
                });

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/reapprovisionnement", dto);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<DemandeReapprovisionnement>();
            Assert.NotNull(result);
            Assert.Equal(123, result!.DemandeId);
            Assert.Equal("EN_ATTENTE", result.Statut);
        }

        [Fact]
        public async Task Get_EnAttente_ShouldReturnOK()
        {
            var response = await _client.GetAsync("/api/v1/reapprovisionnement/en-attente");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
