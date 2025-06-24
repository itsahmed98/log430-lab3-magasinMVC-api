using MagasinMcService.Controllers;
using MagasinMcService.Models;
using MagasinMcService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace MagasinMcService.Test.UnitTest
{
    /// <summary>
    /// Tests unitaires pour le contrôleur MagasinController.
    /// </summary>
    public class MagasinControllerTests
    {
        private readonly Mock<ILogger<MagasinController>> _loggerMock = new();
        private readonly Mock<IMagasinService> _serviceMock = new();

        [Fact]
        public void Constructor_ShouldThrow_WhenDependenciesAreNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new MagasinController(null!, _serviceMock.Object));
            Assert.Throws<ArgumentNullException>(() =>
                new MagasinController(_loggerMock.Object, null!));
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllMagasins()
        {
            _serviceMock.Setup(s => s.GetAllAsync())
                .ReturnsAsync(new List<Magasin> { new() { MagasinId = 1, Nom = "Test" } });

            var controller = new MagasinController(_loggerMock.Object, _serviceMock.Object);
            var result = await controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);
            var magasins = Assert.IsAssignableFrom<List<Magasin>>(ok.Value);
            Assert.Single(magasins);
        }

        [Fact]
        public async Task GetById_ShouldReturnMagasin_WhenExists()
        {
            _serviceMock.Setup(s => s.GetMagasinByIdAsync(1))
                .ReturnsAsync(new Magasin { MagasinId = 1, Nom = "Test" });

            var controller = new MagasinController(_loggerMock.Object, _serviceMock.Object);
            var result = await controller.GetById(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            var magasin = Assert.IsType<Magasin>(ok.Value);
            Assert.Equal("Test", magasin.Nom);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenNotExists()
        {
            _serviceMock.Setup(s => s.GetMagasinByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Magasin?)null);

            var controller = new MagasinController(_loggerMock.Object, _serviceMock.Object);
            var result = await controller.GetById(999);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("non trouvé", notFound.Value!.ToString());
        }
    }
}
