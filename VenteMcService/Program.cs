using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using VenteMcService.Data;
using VenteMcService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<VenteDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IVenteService, VenteService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddHttpClient("ProduitMcService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7198/api/v1/produits");
});

builder.Services.AddHttpClient("StockMcService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7185/api/v1/stocks");
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Vente Service API",
        Version = "v1",
        Description = "API pour gérer les ventes"
    });
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<VenteDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Produit Service API V1");
    });
}
else
{
    app.UseExceptionHandler("/error");  
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseRouting();

app.MapControllers();

app.Run();

public partial class Program { }
