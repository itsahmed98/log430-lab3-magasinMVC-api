namespace MagasinCentral.Models
{
    /// <summary>
    /// Représente un objet de transfert de données (DTO) pour les performances d’un magasin.
    /// </summary>
    public class PerformanceDto
    {
        public int PerformanceId { get; set; }

        public int MagasinId { get; set; }

        public DateTime Date { get; set; }

        public decimal ChiffreAffaires { get; set; }

        public int NbVentes { get; set; }

        public int RupturesStock { get; set; }

        public int Surstock { get; set; }
    }
}
