namespace MagasinCentral.Models
{
    public class VenteDto
    {
        public int VenteId { get; set; }
        public int MagasinId { get; set; }
        public DateTime Date { get; set; }
        public List<LigneVenteDto> Lignes { get; set; } = new();
    }
}
