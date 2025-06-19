using RapportMcService.Models;

namespace RapportMcService.Services
{
    public interface IRapportService
    {
        /// <summary>
        /// Obtient un rapport consolidé de tous les magasins.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<RapportDto>> ObtenirRapportConsolideAsync();
    }
}