using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using StockMcService.Services;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<IStockService> MockStockService { get; } = new();
    public Mock<IInventaireService> MockInventaireService { get; } = new();
    public Mock<IReapprovisionnementService> MockReapprovisionnementService { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Supprimer l'enregistrement réel du service
            services.RemoveAll(typeof(IStockService));
            services.RemoveAll(typeof(IInventaireService));
            services.RemoveAll(typeof(IReapprovisionnementService));

            // Injecter le mock
            services.AddSingleton(MockStockService.Object);
            services.AddSingleton(MockInventaireService.Object);
            services.AddSingleton(MockReapprovisionnementService.Object);
        });
    }
}
