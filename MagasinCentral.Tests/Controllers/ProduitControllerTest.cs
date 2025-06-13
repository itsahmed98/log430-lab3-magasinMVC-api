using MagasinCentral.Controllers;
using MagasinCentral.Data;
using MagasinCentral.Services;
using Moq;
using Microsoft.Extensions.Logging;

namespace MagasinCentral.Tests.Services
{
    public class ProduitControllerTest
    {
        private readonly Mock<ILogger<ProduitController>> _loggerMock;
        private readonly Mock<IProduitService> _produitServiceMock;

        public ProduitControllerTest()
        {
            _loggerMock = new Mock<ILogger<ProduitController>>();
            _produitServiceMock = new Mock<IProduitService>();
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenContextIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ProduitController(null!, _produitServiceMock.Object));
            Assert.Throws<ArgumentNullException>(() => new ProduitController(_loggerMock.Object, null!));
        }

    }
}