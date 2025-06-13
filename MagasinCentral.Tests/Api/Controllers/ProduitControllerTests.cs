using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MagasinCentral.Api.Controllers;
using MagasinCentral.Services;
using MagasinCentral.Models;

namespace MagasinCentral.Api.Tests
{
    /// <summary>
    /// Tests pour le contrôleur Api <see cref="ProduitApiController"/>
    /// </summary>
    public class ProduitControllerTest
    {
        private readonly Mock<IProduitService> _serviceMock;
        private readonly Mock<ILogger<ProduitApiController>> _loggerMock;
        private readonly ProduitApiController _controller;

        public ProduitControllerTest()
        {
            _serviceMock = new Mock<IProduitService>();
            _loggerMock = new Mock<ILogger<ProduitApiController>>();
            _controller = new ProduitApiController(_loggerMock.Object, _serviceMock.Object);
        }

        public void Constructeur_ThrowsArgumentNullException_WhenServiceIsNull()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ProduitApiController(_loggerMock.Object, null!));
            Assert.Throws<ArgumentNullException>(() => new ProduitApiController(null!, _serviceMock.Object));
        }

        [Fact]
        public async Task Produits_ReturnsOk_WithListOfProduits()
        {
            // Arrange
            var liste = new List<Produit>
            {
                new Produit { ProduitId = 1, Nom = "A" },
                new Produit { ProduitId = 2, Nom = "B" }
            };

            _serviceMock.Setup(s => s.GetAllProduitsAsync())
                        .ReturnsAsync(liste);

            // Act
            var result = await _controller.Produits();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(liste, ok.Value);
        }

        [Fact]
        public async Task Produit_ExistingId_ReturnsOk_WithProduit()
        {
            // Arrange
            var prod = new Produit { ProduitId = 3, Nom = "C" };
            _serviceMock.Setup(s => s.GetProduitByIdAsync(3))
                        .ReturnsAsync(prod);

            // Act
            var result = await _controller.Produit(3);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(prod, ok.Value);
        }

        [Fact]
        public async Task Produit_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetProduitByIdAsync(It.IsAny<int>()))
                        .ReturnsAsync((Produit)null);

            // Act
            var result = await _controller.Produit(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Modifier_ValidPayload_ReturnsNoContent()
        {
            // Arrange
            var payload = new Produit { ProduitId = 5, Nom = "D" };
            _serviceMock.Setup(s => s.ModifierProduitAsync(payload))
                        .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Modifier(5, payload);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _serviceMock.Verify(s => s.ModifierProduitAsync(payload), Times.Once);
        }

        [Fact]
        public async Task Modifier_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var payload = new Produit { ProduitId = 7, Nom = "E" };

            // Act
            var result = await _controller.Modifier(8, payload);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("L’ID de l’URL (8) doit correspondre", bad.Value.ToString());
        }

        [Fact]
        public async Task Modifier_NonExisting_ThrowsArgumentException_ReturnsNotFound()
        {
            // Arrange
            var payload = new Produit { ProduitId = 10, Nom = "F" };
            _serviceMock.Setup(s => s.ModifierProduitAsync(payload))
                        .ThrowsAsync(new ArgumentException("Le produit n'existe pas."));

            // Act
            var result = await _controller.Modifier(10, payload);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("Le produit n'existe pas", notFound.Value.ToString());
        }

        [Fact]
        public async Task Modifier_ServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var payload = new Produit { ProduitId = 11, Nom = "G" };
            _serviceMock.Setup(s => s.ModifierProduitAsync(payload))
                        .ThrowsAsync(new InvalidOperationException("Erreur critique."));

            // Act
            var result = await _controller.Modifier(11, payload);

            // Assert
            var error = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, error.StatusCode);
            Assert.Contains("Une erreur s'est produite côté serveur", error.Value.ToString());
        }
    }
}
