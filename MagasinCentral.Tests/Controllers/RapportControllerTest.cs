using System.Collections.Generic;
using System.Threading.Tasks;
using MagasinCentral.Controllers;
using MagasinCentral.Models;
using MagasinCentral.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;

namespace MagasinCentral.Tests
{
    /// <summary>
    /// Tests unitaires pour le RapportController.
    /// </summary>
    public class RapportControllerTest
    {
        private readonly Mock<IRapportService> _rapportServiceMock;
        private readonly Mock<ILogger<RapportController>> _loggerMock;

        public RapportControllerTest()
        {
            _rapportServiceMock = new Mock<IRapportService>();
            _loggerMock = new Mock<ILogger<RapportController>>();
        }

        [Fact]
        public void Constructeur_NullService_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new RapportController(null!, _rapportServiceMock.Object));
            Assert.Throws<ArgumentNullException>(() => new RapportController(_loggerMock.Object, null!));
        }

        [Fact]
        public async Task Index_ReturnsViewWithRapportConsolide()
        {
            // Arrange
            var rapportServiceMock = new Mock<IRapportService>();
            var rapportController = new RapportController(_loggerMock.Object, rapportServiceMock.Object);

            var rapportConsolide = new List<RapportDto>
            {
                new RapportDto
                {
                    NomMagasin = "TestMagasin",
                    ChiffreAffairesTotal = 123.45m,
                    TopProduits = new List<InfosVenteProduit>
                    {
                        new InfosVenteProduit { NomProduit = "X", QuantiteVendue = 5, TotalVentes = 25m }
                    },
                    StocksRestants = new List<InfosStockProduit>
                    {
                        new InfosStockProduit { NomProduit = "X", QuantiteRestante = 45 }
                    }
                }
            };

            rapportServiceMock
                .Setup(s => s.ObtenirRapportConsolideAsync())
                .ReturnsAsync(rapportConsolide);

            // Act
            var result = rapportController.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(rapportConsolide, viewResult.Model);
        }
    }
}
