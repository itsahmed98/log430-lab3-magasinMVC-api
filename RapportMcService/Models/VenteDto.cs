namespace RapportMcService.Models
{
    /// <summary>
    /// Représente une vente
    /// </summary>
    public class VenteDto
    {
        /// <summary>
        /// L'identifiant unique de la vente.
        /// </summary>
        public int VenteId { get; set; }

        /// <summary>
        /// L'identifiant du magasin où la vente a eu lieu.
        /// </summary>
        public int MagasinId { get; set; }

        /// <summary>
        /// La date et l'heure de la vente.
        /// </summary>
        public DateTime Date { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Liste des lignes de vente associées à cette vente.
        /// </summary>
        public List<LigneVenteDto> Lignes { get; set; } = new List<LigneVenteDto>();
    }
}
