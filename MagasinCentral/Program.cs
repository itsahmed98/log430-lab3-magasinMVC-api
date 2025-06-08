using MagasinCentral.Data;
using Microsoft.EntityFrameworkCore;
using MagasinCentral.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MagasinDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IRapportService, RapportService>();
builder.Services.AddScoped<IReapprovisionnementService, ReapprovisionnementService>();
builder.Services.AddScoped<IPerformancesService, PerformancesService>();
builder.Services.AddScoped<IProduitService, ProduitService>();
builder.Services.AddScoped<IVenteService, VenteService>();
builder.Services.AddScoped<IStockService, StockService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Magasin Central API",
        Version = "v1",
        Description = "API for managing the central store operations."
    });
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MagasinDbContext>();
    db.Database.Migrate();
}

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
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
