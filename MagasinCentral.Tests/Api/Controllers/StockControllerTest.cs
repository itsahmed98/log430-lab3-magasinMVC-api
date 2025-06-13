using Moq;
using MagasinCentral.Api.Controllers;
using MagasinCentral.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MagasinCentral.Tests.Api.Controllers
{
    /// <summary>
    /// Tests pour le contrôleur Api <see cref="StockApiController"/>.
    /// </summary>
    public class StockControllerTest
    {
        private readonly Mock<IStockService> _stockServiceMock;
        private readonly Mock<ILogger<StockApiController>> _loggerMock;
        private readonly StockApiController _controller;

        public StockControllerTest()
        {
            _stockServiceMock = new Mock<IStockService>();
            _loggerMock = new Mock<ILogger<StockApiController>>();
            _controller = new StockApiController(_loggerMock.Object, _stockServiceMock.Object);
        }

        [Fact]
        public void Constructeur_ThrowsArgumentNullException_WhenServiceIsNull()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => new StockApiController(_loggerMock.Object, null!));
            Assert.Throws<ArgumentNullException>(() => new StockApiController(null!, _stockServiceMock.Object));
        }

        [Fact]
        public async Task GetStockMagasin_WithValidMagasinId_ShouldReturnQuantiteStock()
        {
            // Arrange
            int magasinId = 1;
            int expectedStock = 100;

            _stockServiceMock.Setup(s => s.GetStockByMagasinId(magasinId))
                .ReturnsAsync(expectedStock);

            // Act
            var result = await _controller.GetStockMagasin(magasinId);

            // Assert
            Assert.Equal(expectedStock, result.Value);
        }

        [Fact]
        public async Task GetStockMagasin_WithInvalidMagasinId_ShouldReturnNotFound()
        {
            // Arrange
            int magasinId = 999; // Assuming this ID does not exist
            _stockServiceMock.Setup(s => s.GetStockByMagasinId(magasinId))
                .ReturnsAsync((int?)null);

            // Act
            var result = await _controller.GetStockMagasin(magasinId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<int>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }
    }
}