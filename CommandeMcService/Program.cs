using CommandeMcService.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
builder.Services.AddHttpClient("StockMcService", client =>
{
    client.BaseAddress = new Uri(config["Services:Stock"]);
});

builder.Services.AddHttpClient("PanierMcService", client =>
{
    client.BaseAddress = new Uri(config["Services:Panier"]);
});

builder.Services.AddHttpClient("VenteMcService", client =>
{
    client.BaseAddress = new Uri(config["Services:Vente"]);
});

builder.Services.AddScoped<ICommandeService, CommandeService>();

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
        Title = "Commandes Service API",
        Version = "v1",
        Description = "API pour la validation des commandes en ligne"
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

public partial class Program { }