

// public class ProduitController : Controller
// {
//     private readonly HttpClient _http;

//     public ProduitController(HttpClient http)
//     {
//         _http = http;
//     }

//     public async Task<IActionResult> Index()
//     {
//         var response = await _http.GetFromJsonAsync<List<Produit>>("http://produit-service/api/produits");
//         return View(response);
//     }
// }