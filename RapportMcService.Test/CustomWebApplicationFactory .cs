using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using RapportMcService.Services;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<IRapportService> MockRapportService { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Supprimer l'enregistrement réel du service
            services.RemoveAll(typeof(IRapportService));

            // Injecter le mock
            services.AddSingleton(MockRapportService.Object);
        });
    }
}
