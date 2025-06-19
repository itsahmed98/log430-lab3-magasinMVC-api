namespace RapportMcService.Models
{
    /// <summary>
    /// Représente un rapport de vente pour un magasin.
    /// </summary>
    public class RapportDto
    {
        /// <summary>
        /// Le nom du magasin.
        /// </summary>
        public string NomMagasin { get; init; } = string.Empty;

        /// <summary>
        /// Le chiffre d'affaires total du magasin.
        /// </summary>
        public decimal ChiffreAffairesTotal { get; init; }

        /// <summary>
        /// Les trois produits les plus vendus dans le magasin.
        /// </summary>
        public List<InfosVenteProduit> TopProduits { get; init; } = new();

        /// <summary>
        /// Les stocks restants pour les produits dans le magasin.
        /// </summary>
        public List<InfosStockProduit> StocksRestants { get; init; } = new();
    }
}