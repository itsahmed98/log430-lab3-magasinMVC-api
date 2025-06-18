using Microsoft.EntityFrameworkCore;
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
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var ligne in vente.Lignes)
                {
                    var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/{ligne.ProduitId}");
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new InvalidOperationException($"Produit ID={ligne.ProduitId} introuvable.");
                    }
                }

                var lignes = vente.Lignes.ToList();
                vente.Lignes.Clear();

                _context.Ventes.Add(vente);
                await _context.SaveChangesAsync();

                foreach (var ligne in lignes)
                {
                    ligne.VenteId = vente.VenteId;
                    _context.LignesVente.Add(ligne);
                }
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return await GetByIdAsync(vente.VenteId) ?? vente;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erreur lors de la création de la vente.");
                throw;
            }
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
