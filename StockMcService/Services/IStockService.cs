using StockMcService.Models;

namespace StockMcService.Services
{
    public interface IStockService
    {
        /// <summary>
        /// Retrouve tous les stocks disponibles.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<StockDto>> GetAllStocksAsync();

        /// <summary>
        /// Retourne le stock d'un produit spécifique dans un magasin donné.
        /// </summary>
        /// <param name="magasinId"></param>
        /// <param name="produitId"></param>
        /// <returns></returns>
        Task<StockDto?> GetStockByMagasinProduitAsync(int magasinId, int produitId);

        /// <summary>
        /// Récupère le stock d'un magasin spécifique.
        /// </summary>
        /// <param name="magasinId"></param>
        /// <returns></returns>
        Task<IEnumerable<StockDto>> GetStockByMagasinAsync(int magasinId);
    }
}