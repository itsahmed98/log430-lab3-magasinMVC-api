using MagasinCentral.Controllers;
using MagasinCentral.Data;
using MagasinCentral.Services;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MagasinCentral.Tests.Controllers
{
    /// <summary>
    /// Tests unitaires pour le contrôleur VenteController.
    /// </summary>
    public class VenteControllerTest
    {
        private readonly Mock<IVenteService> _venteServiceMock;
        private readonly Mock<IProduitService> _produitServiceMock;
        private readonly MagasinDbContext _contexte;
        private readonly Mock<ILogger<VenteController>> _loggerMock;

        public VenteControllerTest()
        {
            _venteServiceMock = new Mock<IVenteService>();
            _produitServiceMock = new Mock<IProduitService>();
            _loggerMock = new Mock<ILogger<VenteController>>();

            var options = new DbContextOptionsBuilder<MagasinDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_For_Constructor")
            .Options;
            _contexte = new MagasinDbContext(options);
        }

        [Fact]
        public void Constructeur_NullServices_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new VenteController(null!, _venteServiceMock.Object, _produitServiceMock.Object, _contexte));
            Assert.Throws<ArgumentNullException>(() => new VenteController(_loggerMock.Object, null!, _produitServiceMock.Object, _contexte));
            Assert.Throws<ArgumentNullException>(() => new VenteController(_loggerMock.Object, _venteServiceMock.Object, null!, _contexte));
            Assert.Throws<ArgumentNullException>(() => new VenteController(_loggerMock.Object, _venteServiceMock.Object, _produitServiceMock.Object, null!));
        }

    }
}