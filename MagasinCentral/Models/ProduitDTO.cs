namespace MagasinCentral.Models
{
    public class ProduitDto
    {
        public int ProduitId { get; set; }
        public string Nom { get; set; } = null!;
        public string? Categorie { get; set; }
        public decimal Prix { get; set; }
        public string? Description { get; set; }
    }
}
