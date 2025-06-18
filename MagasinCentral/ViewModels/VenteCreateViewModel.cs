using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MagasinCentral.ViewModels
{
    public class VenteCreateViewModel
    {
        [Required]
        public int MagasinId { get; set; }

        // Pour l’UI, on pourra initialiser 2 lignes par défaut
        public List<LigneViewModel> Lignes { get; set; } = new()
        {
            new LigneViewModel(),
            new LigneViewModel()
        };
    }
}
