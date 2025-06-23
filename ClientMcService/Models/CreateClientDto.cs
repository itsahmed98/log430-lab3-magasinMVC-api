using System.ComponentModel.DataAnnotations;

namespace ClientMcService.Models
{
    public class CreateClientDto
    {
        public string Nom { get; set; } = null!;

        [EmailAddress]
        public string Courriel { get; set; } = null!;

        public string? Adresse { get; set; }
    }
}
