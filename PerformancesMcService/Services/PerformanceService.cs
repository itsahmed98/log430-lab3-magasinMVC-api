using Microsoft.EntityFrameworkCore;
using PerformancesMcService.Data;
using PerformancesMcService.Models;

namespace PerformancesMcService.Services
{
    public class PerformanceService : IPerformanceService
    {
        private readonly PerformanceDbContext _context;
        private readonly ILogger<PerformanceService> _logger;

        public PerformanceService(ILogger<PerformanceService> logger, PerformanceDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Performance>> GetAllPerformancesAsync()
        {
            return await _context.Performances
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
