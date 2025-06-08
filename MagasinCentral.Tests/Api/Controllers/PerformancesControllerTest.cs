using MagasinCentral.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MagasinCentral.ViewModels;

namespace MagasinCentral.Api.Controllers
{
    /// <summary>
    /// Test pour le contrôleur Api <see cref="PerformanceController"/>
    /// </summary>
    public class PerformancesControllerTests
    {
        private readonly Mock<IPerformancesService> _performancesServiceMock;
        private readonly Mock<ILogger<PerformancesController>> _loggerMock;
        private readonly PerformancesController _controller;

        public PerformancesControllerTests()
        {
            _performancesServiceMock = new Mock<IPerformancesService>();
            _loggerMock = new Mock<ILogger<PerformancesController>>();
            _controller = new PerformancesController(_loggerMock.Object, _performancesServiceMock.Object);
        }

        /// <summary>
        /// Test du constructeur pour vérifier qu'il lance une exception ArgumentNullException
        /// </summary>
        [Fact]
        public void Constructeur_ThrowsArgumentNullException_WhenServiceIsNull()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => new PerformancesController(_loggerMock.Object, null!));
            Assert.Throws<ArgumentNullException>(() => new PerformancesController(null!, _performancesServiceMock.Object));
        }

        /// <summary>
        /// Test de la méthode GetPerformances pour vérifier qu'elle renvoie un résultat Ok avec les données attendues.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetPerformances_ShouldReturnOk()
        {
            // Arrange
            var expectedVm = new PerformancesViewModel
            {
                RevenusParMagasin = new List<RevenuMagasin>
                {
                    new RevenuMagasin { MagasinId = 1, NomMagasin = "M1", ChiffreAffaires = 100m }
                },
                ProduitsRupture = new List<StockProduitLocal>(),
                ProduitsSurstock = new List<StockProduitLocal>(),
                TendancesHebdomadairesParMagasin = new Dictionary<int, List<VentesQuotidiennes>>()
            };

            _performancesServiceMock
                .Setup(s => s.GetPerformances())
                .ReturnsAsync(expectedVm);

            // Act
            var actionResult = await _controller.GetPerformances();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var actualVm = Assert.IsType<PerformancesViewModel>(okResult.Value);
            Assert.Same(expectedVm, actualVm);
        }
    }
}