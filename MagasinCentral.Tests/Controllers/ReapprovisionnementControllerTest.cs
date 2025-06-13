using MagasinCentral.Services;
using MagasinCentral.Controllers;
using MagasinCentral.ViewModels;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MagasinCentral.Tests.Controllers
{
    /// <summary>
    /// Tests unitaires pour le contrôleur de réapprovisionnement.
    /// </summary>
    public class ReapprovisionnementControllerTest
    {
        private readonly Mock<IReapprovisionnementService> _reapprovisionnementServiceMock;
        private readonly Mock<ILogger<ReapprovisionnementController>>? _loggerMock;

        public ReapprovisionnementControllerTest()
        {
            _reapprovisionnementServiceMock = new Mock<IReapprovisionnementService>();
            _loggerMock = new Mock<ILogger<ReapprovisionnementController>>();
        }

        /// <summary>
        /// Test du constructeur du contrôleur de réapprovisionnement avec un service null.
        /// </summary>
        [Fact]
        public void Constructeur_NullService_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ReapprovisionnementController(_loggerMock.Object, null!));
            Assert.Throws<ArgumentNullException>(() => new ReapprovisionnementController(null!, _reapprovisionnementServiceMock.Object));
        }

        /// <summary>
        /// Test de la méthode Index pour vérifier qu'elle retourne la vue avec les stocks.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Index_ReturnsViewWithStocks()
        {
            // Arrange
            var reapprovisionnementServiceMock = new Mock<IReapprovisionnementService>();
            var controller = new ReapprovisionnementController(_loggerMock.Object, reapprovisionnementServiceMock.Object);

            int magasinId = 1;
            var stocks = new List<StockVue>
            {
                new StockVue { ProduitId = 1, NomProduit = "Produit A", QuantiteLocale = 10, QuantiteCentral = 50 },
                new StockVue { ProduitId = 2, NomProduit = "Produit B", QuantiteLocale = 5, QuantiteCentral = 30 }
            };

            reapprovisionnementServiceMock
                .Setup(s => s.GetStocksAsync(magasinId))
                .ReturnsAsync(stocks);

            // Act
            var result = await controller.Index(magasinId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(stocks, viewResult.Model);
        }
    }
}