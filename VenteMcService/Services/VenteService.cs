using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using VenteMcService.Data;
using VenteMcService.Models;

namespace VenteMcService.Services
{
    public class VenteService : IVenteService
    {
        private readonly VenteDbContext _context;
        private readonly ILogger<VenteService> _logger;
        private readonly HttpClient _httpClient;

        public VenteService(ILogger<VenteService> logger, VenteDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClientFactory?.CreateClient("ProduitMcService") ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        /// <inheritdoc/> 
        public async Task<Vente> CreateAsync(Vente vente)
        {
            foreach (var ligneDto in vente.Lignes)
            {
                var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/{ligneDto.ProduitId}");
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Produit {ligneDto.ProduitId} non trouvé.");

                var json = await response.Content.ReadAsStringAsync();
                var produit = JsonSerializer.Deserialize<ProduitDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                vente.Lignes.Add(new LigneVente
                {
                    ProduitId = ligneDto.ProduitId,
                    Quantite = ligneDto.Quantite,
                    PrixUnitaire = produit.Prix
                });
            }

            _context.Ventes.Add(vente);
            await _context.SaveChangesAsync();
            return vente;
        }

        /// <inheritdoc/> 
        public async Task DeleteAsync(int id)
        {
            var exist = await _context.Ventes
                .Include(v => v.Lignes)
                .FirstOrDefaultAsync(v => v.VenteId == id);

            if (exist == null)
                throw new KeyNotFoundException($"Vente ID={id} introuvable.");

            _context.LignesVente.RemoveRange(exist.Lignes);
            _context.Ventes.Remove(exist);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/> 
        public async Task<List<Vente>> GetAllAsync()
        {
            return await _context.Ventes
                .Include(v => v.Lignes)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <inheritdoc/> 
        public async Task<Vente?> GetByIdAsync(int id)
        {
            return await _context.Ventes
                .Include(v => v.Lignes)
                .FirstOrDefaultAsync(v => v.VenteId == id);
        }

        /// <inheritdoc/>
        public async Task<List<Vente>> GetByMagasinAsync(int magasinId)
        {
            return await _context.Ventes
                .Where(v => v.MagasinId == magasinId)
                .Include(v => v.Lignes)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
