using MagasinMcService.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace MagasinMcService.Test
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public Mock<IMagasinService> MockMagasinService { get; } = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(IMagasinService));
                services.AddSingleton(MockMagasinService.Object);
            });
        }
    }
}
