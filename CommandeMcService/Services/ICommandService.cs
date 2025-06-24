using CommandeMcService.Models;

namespace CommandeMcService.Services;

/// <summary>
/// Définition du service pour gérer les commandes.
/// </summary>
public interface ICommandeService
{
    /// <summary>
    /// Valider une commande en vérifiant les détails et la disponibilité des produits.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<bool> ValiderCommandeAsync(CommandeValidationDto dto);
}
