using System.Net;

namespace StockMcService.Test.IntegrationTests
{
    public class StockControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public StockControllerIntegrationTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ShouldReturnOK()
        {
            var response = await _client.GetAsync("/api/v1/stocks");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetOne_ShouldReturnNotFound_WhenInvalid()
        {
            var response = await _client.GetAsync("/api/v1/stocks/999/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
