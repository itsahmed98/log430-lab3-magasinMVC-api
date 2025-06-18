using ProduitMcService.Models;

namespace ProduitMcService.Services
{
    /// <summary>
    /// Contrat du service gestion produit.
    /// </summary>
    public interface IProduitService
    {
        /// <summary>
        /// Retourner tous les produits
        /// </summary>
        /// <returns>List des produits</returns>
        Task<List<Produit>> GetAllAsync();

        /// <summary>
        /// Retourner un produit par son ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Un Produit</returns>
        Task<Produit?> GetByIdAsync(int id);

        /// <summary>
        /// Créer un nouveau produit.
        /// </summary>
        /// <param name="produit"></param>
        /// <returns>Produit</returns>
        Task<Produit> CreateAsync(Produit produit);

        /// <summary>
        /// Mettre à jour un produit existant.
        /// </summary>
        /// <param name="produit"></param>
        Task UpdateAsync(Produit produit);

        /// <summary>
        /// Supprimer un produit par son ID.
        /// </summary>
        /// <param name="id"></param>
        Task DeleteAsync(int id);
    }
}
