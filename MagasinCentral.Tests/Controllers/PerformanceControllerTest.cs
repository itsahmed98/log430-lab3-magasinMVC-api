using MagasinCentral.Controllers;
using MagasinCentral.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MagasinCentral.ViewModels;
using Microsoft.Extensions.Logging;


namespace MagasinCentral.Tests.Services
{
    /// <summary>
    /// Tests unitaires pour le PerformancesController.
    /// </summary>
    public class PerformanceControllerTest
    {
        private readonly Mock<IPerformancesService>? _serviceMock;
        private readonly Mock<ILogger<PerformancesController>>? _loggerMock;

        public PerformanceControllerTest()
        {
            _serviceMock = new Mock<IPerformancesService>();
            _loggerMock = new Mock<ILogger<PerformancesController>>();
        }

        /// <summary>
        /// Initialise les mocks pour les tests.
        /// </summary>
        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenServiceIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new PerformancesController(_serviceMock!.Object, null!));
            Assert.Throws<ArgumentNullException>(() => new PerformancesController(null!, _loggerMock!.Object));
        }

        /// <summary>
        /// Teste la méthode Index du PerformancesController.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Index_ShouldReturnViewWithPerformanceData()
        {
            // Arrange
            var serviceMock = new Mock<IPerformancesService>();
            var controller = new PerformancesController(serviceMock.Object, _loggerMock!.Object);

            var performanceData = new PerformancesViewModel();
            serviceMock.Setup(s => s.GetPerformances()).ReturnsAsync(performanceData);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(performanceData, viewResult.Model);
        }
    }
}