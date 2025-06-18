using MagasinCentral.Data;
using MagasinCentral.Models;
using MagasinCentral.Services;
using MagasinCentral.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace MagasinCentral.Controllers;

/// <summary>
/// Controller pour gérer les ventes.
/// </summary>
public class VenteController : Controller
{
    private readonly MagasinDbContext _contexte;
    private readonly ILogger<VenteController> _logger;
    private readonly HttpClient _httpClientVente;
    private readonly HttpClient _httpClientProduit;


    /// <summary>
    /// Constructeur pour initialiser les services nécessaires à la gestion des ventes.
    /// </summary>
    /// <param name="venteService"></param>
    /// <param name="produitService"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public VenteController(ILogger<VenteController> logger, 
        IHttpClientFactory httpClientFactoryVente,
        IHttpClientFactory httpClientFactoryProduit, 
        MagasinDbContext contexte)
    {
        _httpClientVente = httpClientFactoryVente?.CreateClient("VenteMcService") ?? throw new ArgumentNullException(nameof(httpClientFactoryVente));
        _httpClientProduit = httpClientFactoryProduit?.CreateClient("ProduitMcService") ?? throw new ArgumentNullException(nameof(httpClientFactoryProduit));
        _contexte = contexte ?? throw new ArgumentNullException(nameof(contexte));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Affiche le formulaire pour enregistrer une vente dans un magasin spécifique.
    /// </summary>
    /// <param name="magasinId"></param>
    public async Task<IActionResult> Enregistrer(int? magasinId)
    {
        // Récupère la liste des magasins pour un <select>
        var magasins = await _contexte.Magasins
            .Select(m => new { m.MagasinId, m.Nom })
            .ToListAsync();
        ViewBag.Magasins = new SelectList(magasins, "MagasinId", "Nom");

        // Récupère la liste des produits pour un <select>
        var responseP = await _httpClientProduit.GetAsync("");
        var produits = await responseP.Content.ReadFromJsonAsync<List<ProduitDto>>();
        ViewBag.Produits = new SelectList(produits, "ProduitId", "Nom");

        // Initialise le ViewModel
        var vm = new VenteCreateViewModel
        {
            MagasinId = magasinId ?? 0,
            Lignes = new List<LigneViewModel>
        {
            new LigneViewModel(),
            new LigneViewModel()
        }
        };

        return View(vm);
    }

    /// <summary>
    /// Enregistre une vente pour un magasin donné avec les produits et quantités spécifiés.
    /// </summary>
    /// <param name="magasinId"></param>
    /// <param name="produitId"></param>
    /// <param name="quantite"></param>
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Enregistrer(VenteCreateViewModel vm)
    {
        // Re-peupler les listes en cas de retour à la vue
        var magasins = await _contexte.Magasins
            .Select(m => new { m.MagasinId, m.Nom })
            .ToListAsync();
        ViewBag.Magasins = new SelectList(magasins, "MagasinId", "Nom");
        var responseP = await _httpClientProduit.GetAsync("");
        var produits = await responseP.Content.ReadFromJsonAsync<List<ProduitDto>>();
        ViewBag.Produits = new SelectList(produits, "ProduitId", "Nom");

        if (!ModelState.IsValid)
            return View(vm);

        // Construire le payload JSON que la micro-API attend
        var payload = new
        {
            MagasinId = vm.MagasinId,
            Date = DateTime.UtcNow,
            Lignes = vm.Lignes
                              .Where(l => l.Quantite > 0)
                              .Select(l => new {
                                  ProduitId = l.ProduitId,
                                  Quantite = l.Quantite
                              })
                              .ToList()
        };

        var response = await _httpClientVente
            .PostAsJsonAsync("", payload);

        if (response.IsSuccessStatusCode)
        {
            // Récupérer l’ID créé
            var created = await response.Content.ReadFromJsonAsync<JsonElement>();
            int venteId = created.GetProperty("venteId").GetInt32();
            TempData["Succès"] = $"Vente #{venteId} créée.";
            return RedirectToAction(nameof(Liste));
        }

        // En cas d’erreur, afficher le message
        var error = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError("", $"API Erreur : {error}");
        return View(vm);
    }

    /// <summary>
    /// Annule une vente existante et restitue le stock au magasin.
    /// </summary>
    /// <param name="venteId"></param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Retour(int venteId)
    {
        var response = await _httpClientVente.DeleteAsync($"{_httpClientVente.BaseAddress}/{venteId}");

        if (response.IsSuccessStatusCode)
        {
            TempData["Succès"] = "Vente annulée avec succès.";
        }
        else
        {
            TempData["Erreur"] = "Erreur lors de l’annulation de la vente.";
        }

        return RedirectToAction(nameof(Liste));
    }

    /// <summary>
    /// Affiche la liste de toutes les ventes enregistrées, incluant les informations sur le magasin et les produits.
    /// </summary>
    public async Task<IActionResult> Liste()
    {
        var response = await _httpClientVente.GetAsync("");
        if (!response.IsSuccessStatusCode)
        {
            TempData["Erreur"] = "Impossible de charger les ventes.";
            return View(new List<VenteDto>());
        }

        var json = await response.Content.ReadAsStringAsync();
        var ventes = JsonSerializer.Deserialize<List<VenteDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(ventes);
    }


}
