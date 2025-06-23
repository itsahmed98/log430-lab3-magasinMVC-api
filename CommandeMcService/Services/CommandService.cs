using CommandeMcService.Models;

namespace CommandeMcService.Services;

public class CommandeService : ICommandeService
{
    private readonly HttpClient _httpPanier;
    private readonly HttpClient _httpStock;
    private readonly HttpClient _httpClientVente;
    private readonly ILogger<CommandeService> _logger;

    public CommandeService(ILogger<CommandeService> logger, IHttpClientFactory factory)
    {
        _httpPanier = factory.CreateClient("PanierMcService");
        _httpStock = factory.CreateClient("StockMcService");
        _httpClientVente = factory.CreateClient("VenteMcService");
        _logger = logger;
    }

    public async Task<bool> ValiderCommandeAsync(CommandeValidationDto dto)
    {
        foreach (var ligne in dto.Lignes)
        {
            var stock = await _httpStock.GetFromJsonAsync<StockDto>($"{_httpStock.BaseAddress}/1/{ligne.ProduitId}");

            if (stock is null || stock.Quantite < ligne.Quantite)
            {
                _logger.LogWarning("Produit ID {ProduitId} non disponible en stock pour la quantité demandée.", ligne.ProduitId);
                return false;
            }
        }

        var payload = new
        {
            MagasinId = 1, // stock central
            ClientId = dto.ClientId,
            IsEnLigne = true,
            Date = DateTime.UtcNow,
            Lignes = dto.Lignes.Select(l => new
            {
                l.ProduitId,
                l.Quantite
            }).ToList()
        };

        var response = await _httpClientVente.PostAsJsonAsync("", payload);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Erreur lors de la création de la vente pour la commande du client ID {ClientId}", dto.ClientId);
            return false;
        }

        _logger.LogInformation("Commande validée avec succès pour le client ID {ClientId}", dto.ClientId);
        return true;
    }
}