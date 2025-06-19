using Microsoft.OpenApi.Models;
using RapportMcService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHttpClient("ProduitMcService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7198/api/v1/produits"); // a remplacer par le gateway: http://gateway/api/produits/
});

builder.Services.AddHttpClient("VenteMcService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7184/api/v1/ventes");
});

builder.Services.AddHttpClient("StockMcService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7185/api/v1/stocks");
});

builder.Services.AddScoped<IRapportService, RapportService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Rapport Service API",
        Version = "v1",
        Description = "API pour gérer les rapports consolidés des magasins"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
