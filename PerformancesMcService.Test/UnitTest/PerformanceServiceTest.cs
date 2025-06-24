using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using PerformancesMcService.Data;
using PerformancesMcService.Models;
using PerformancesMcService.Services;
using Xunit;

namespace PerformancesMcService.Test.UnitTests.Services
{
    public class PerformanceServiceTest
    {
        private readonly PerformanceService _service;
        private readonly PerformanceDbContext _context;

        public PerformanceServiceTest()
        {
            var options = new DbContextOptionsBuilder<PerformanceDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new PerformanceDbContext(options);
            _context.Performances.AddRange(
                new Performance { PerformanceId = 1, MagasinId = 1, ChiffreAffaires = 100, Date = DateTime.UtcNow },
                new Performance { PerformanceId = 2, MagasinId = 2, ChiffreAffaires = 200, Date = DateTime.UtcNow }
            );
            _context.SaveChanges();

            var loggerMock = new Mock<ILogger<PerformanceService>>();
            _service = new PerformanceService(loggerMock.Object, _context);
        }

        [Fact]
        public void Constructeur_ShouldThrowArgumentNullException_WhenLoggerIsNull()
        {
            var loggerMock = new Mock<ILogger<PerformanceService>>();
            Assert.Throws<ArgumentNullException>(() => new PerformanceService(null!, _context));
            Assert.Throws<ArgumentNullException>(() => new PerformanceService(loggerMock.Object, null!));
        }

        [Fact]
        public async Task GetAllPerformancesAsync_ReturnsAll()
        {
            var result = await _service.GetAllPerformancesAsync();
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }
    }
}
