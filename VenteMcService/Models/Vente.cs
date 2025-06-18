using System.ComponentModel.DataAnnotations;

namespace VenteMcService.Models
{
    /// <summary>
    /// Représente une vente. 
    /// </summary>
    public class Vente
    {
        /// <summary>
        /// L'identifiant unique de la vente.
        /// </summary>
        [Key]
        public int VenteId { get; set; }

        /// <summary>
        /// L'identifiant du magasin où la vente a eu lieu.
        /// </summary>
        [Required]
        public int MagasinId { get; set; }

        /// <summary>
        /// La date et l'heure de la vente.
        /// </summary>
        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Liste des lignes de vente associées à cette vente.
        /// </summary>
        public List<LigneVente> Lignes { get; set; } = new List<LigneVente>();
    }
}
