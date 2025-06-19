namespace StockMcService.Models
{
    /// <summary>
    /// Représente la quantité en stock d’un produit dans un magasin.
    /// </summary>
    public class StockItem
    {
        /// <summary>
        /// L'identifiant unique du magasin.
        /// </summary>
        public int MagasinId { get; set; }

        /// <summary>
        /// L'identifiant unique du produit.
        /// </summary>
        public int ProduitId { get; set; }

        /// <summary>
        /// La quantité de produit disponible en stock dans le magasin.
        /// </summary>
        public int Quantite { get; set; }
    }
}