using Microsoft.OpenApi.Models;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("ProduitMcService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7198/api/v1/produits"); // a remplacer par le gateway
});

builder.Services.AddHttpClient("PerformancesMcService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7044/api/v1/performances");
});

builder.Services.AddHttpClient("VenteMcService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7184/api/v1/ventes");
});

builder.Services.AddHttpClient("MagasinMcService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7013/api/v1/magasins");
});

builder.Services.AddHttpClient("StockMcService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7185");
});

builder.Services.AddHttpClient("RapportMcService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7214/api/v1/rapports");
});

builder.Services.AddHttpClient("ClientMcService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7041/api/v1/clients");
});

builder.Services.AddHttpClient("PanierMcService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7019/api/v1/panier"); 
});

builder.Services.AddHttpClient("CommandeMcService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7154/api/v1/commandes/valider");
});

builder.Services.AddControllersWithViews();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Magasin Central API",
        Version = "v1",
        Description = "API pour la gestion central des magasins."
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:5252")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

builder.Services.AddResponseCaching();
builder.Services.AddMemoryCache();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Magasin Central API V1");
        c.RoutePrefix = "swagger";
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseCors("AllowFrontend");

app.UseHttpMetrics(); // Middleware pour les m�triques HTTP
app.MapMetrics();
app.UseResponseCaching();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
