namespace StockMcService.Models
{
    /// <summary>
    /// DTO pour exposer le stock via l’API.
    /// </summary>
    public class StockDto
    {
        public int MagasinId { get; set; }
        public int ProduitId { get; set; }
        public int Quantite { get; set; }
    }
}