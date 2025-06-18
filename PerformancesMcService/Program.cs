using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PerformancesMcService.Data;
using PerformancesMcService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<PerformanceDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IPerformanceService, PerformanceService>();

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
        Title = "Performances Service API",
        Version = "v1",
        Description = "Microservice pour récupérer des indicateurs de performance"
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PerformanceDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Performances API V1");
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