namespace RapportMcService.Models
{
    public class InfosVenteProduit
    {
        public string NomProduit { get; set; } = string.Empty;
        public int QuantiteVendue { get; set; }
        public decimal TotalVentes { get; set; }
    }
}