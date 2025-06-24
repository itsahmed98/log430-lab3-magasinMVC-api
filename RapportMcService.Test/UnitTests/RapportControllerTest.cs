using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using RapportMcService.Controllers;
using RapportMcService.Models;
using RapportMcService.Services;

public class RapportControllerTest
{
    private readonly Mock<IRapportService> _mockService = new();
    private readonly Mock<ILogger<RapportController>> _mockLogger = new();

    [Fact]
    public void Constructeur_WithNullLogger_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RapportController(null!, _mockService.Object));
    }

    [Fact]
    public void Constructeur_WithNullService_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RapportController(_mockLogger.Object, null!));
    }

    [Fact]
    public async Task GetRapport_WhenRapportIsReturned_ShouldReturnOkResult()
    {
        // Arrange
        var expected = new RapportVentesDto
        {
            DateGeneration = DateTime.UtcNow,
            VentesParMagasin = new(),
            ProduitsLesPlusVendus = new(),
            StocksRestants = new()
        };

        _mockService.Setup(s => s.ObtenirRapportConsolideAsync()).ReturnsAsync(expected);

        var controller = new RapportController(_mockLogger.Object, _mockService.Object);

        // Act
        var result = await controller.GetRapport();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actual = Assert.IsType<RapportVentesDto>(okResult.Value);
        Assert.Equal(expected.DateGeneration, actual.DateGeneration);
    }

    [Fact]
    public async Task GetRapport_WhenRapportIsNull_ShouldReturnNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.ObtenirRapportConsolideAsync()).ReturnsAsync((RapportVentesDto?)null);

        var controller = new RapportController(_mockLogger.Object, _mockService.Object);

        // Act
        var result = await controller.GetRapport();

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("Aucun rapport disponible.", notFoundResult.Value);
    }
}