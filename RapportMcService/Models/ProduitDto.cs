namespace RapportMcService.Models
{
    /// <summary>
    /// Produit récupéré depuis ProduitMcService (en lecture uniquement).
    /// </summary>
    public class ProduitDto
    {
        /// <summary>
        /// L'identifiant unique du produit.
        /// </summary>
        public int ProduitId { get; set; }

        /// <summary>
        /// Le nom du produit.
        /// </summary>
        public string Nom { get; set; } = "";
    }
}
