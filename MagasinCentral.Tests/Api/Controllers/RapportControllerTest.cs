using MagasinCentral.Api.Controllers;
using MagasinCentral.Models;
using MagasinCentral.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace MagasinCentral.Tests.Api.Controllers
{
    /// <summary>
    /// Tests pour le contrôleur Api <see cref="RapportApiController"/>.
    /// </summary>
    public class RapportControllerTests
    {
        private readonly Mock<IRapportService> _rapportServiceMock;
        private readonly Mock<ILogger<IRapportService>> _loggerMock;
        private readonly RapportApiController _controller;

        public RapportControllerTests()
        {
            _rapportServiceMock = new Mock<IRapportService>();
            _loggerMock = new Mock<ILogger<IRapportService>>();
            _controller = new RapportApiController(_loggerMock.Object, _rapportServiceMock.Object);
        }

        [Fact]
        public void Constructeur_ThrowsArgumentNullException_WhenServiceIsNull()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => new RapportApiController(_loggerMock.Object, null!));
            Assert.Throws<ArgumentNullException>(() => new RapportApiController(null!, _rapportServiceMock.Object));
        }

        [Fact]
        public async Task RapportConsolide_ReturnsOk_WithListOfRapportDto()
        {
            // Arrange
            var expectedData = new List<RapportDto>
            {
                new RapportDto { NomMagasin = "Mag1", ChiffreAffairesTotal = 100 },
                new RapportDto { NomMagasin = "Mag2", ChiffreAffairesTotal = 200 }
            };

            _rapportServiceMock
                .Setup(s => s.ObtenirRapportConsolideAsync())
                .ReturnsAsync(expectedData); // Moq le service pour retourner des données simulées

            // Act
            var actionResult = await _controller.RapportConsolide();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(actionResult);
            var returned = Assert.IsAssignableFrom<IEnumerable<RapportDto>>(ok.Value);
            Assert.Collection(returned,
                dto => Assert.Equal("Mag1", dto.NomMagasin),
                dto => Assert.Equal("Mag2", dto.NomMagasin));
        }

        [Fact]
        public async Task RapportConsolide_ServerError_ShouldReturnStatusCode500()
        {
            // Arrange
            _rapportServiceMock
                .Setup(s => s.ObtenirRapportConsolideAsync())
                .ThrowsAsync(new InvalidOperationException("Oops"));

            // Act
            var actionResult = await _controller.RapportConsolide();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(500, statusCodeResult.StatusCode);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, _) =>
                        state.ToString()!.Contains("Erreur lors de la récupération du rapport consolidé.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
