using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using PerformancesMcService.Controllers;
using PerformancesMcService.Models;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace PerformancesMcService.Test.IntegrationTest
{
    public class PerformanceControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public PerformanceControllerTest(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenHttpClientFactoryIsNull()
        {
            var loggerMock = new Mock<ILogger<PerformanceController>>();
            Assert.Throws<ArgumentNullException>(() => new PerformanceController(loggerMock.Object, null!));
        }

        [Fact]
        public async Task GetAll_ReturnsOkAndList()
        {
            var response = await _client.GetAsync("/api/v1/performances");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var performances = await response.Content.ReadFromJsonAsync<List<Performance>>();
            Assert.NotNull(performances);
            Assert.True(performances.Count >= 1);
        }
    }
}
