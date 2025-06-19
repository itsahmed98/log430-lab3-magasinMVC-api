namespace RapportMcService.Models
{
    /// <summary>
    /// Représente le stock d’un produit dans un magasin
    /// </summary>
    public class StockDto
    {
        public int MagasinId { get; set; }
        public int ProduitId { get; set; }
        public int Quantite { get; set; }
    }
}
