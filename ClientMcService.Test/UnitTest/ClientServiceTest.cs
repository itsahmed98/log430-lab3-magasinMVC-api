using ClientMcService.Data;
using ClientMcService.Models;
using ClientMcService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

public class ClientServiceTest
{
    private readonly ClientDbContext _context;
    private readonly ClientService _service;

    public ClientServiceTest()
    {
        var options = new DbContextOptionsBuilder<ClientDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ClientDbContext(options);
        var logger = new Mock<ILogger<ClientService>>();
        _service = new ClientService(logger.Object, _context);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddClientAndReturnDto()
    {
        var dto = new CreateClientDto { Nom = "Alice", Courriel = "a@a.com", Adresse = "MTL" };

        var result = await _service.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.True(result.ClientId > 0);
        Assert.Equal("Alice", result.Nom);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnClients()
    {
        _context.Clients.Add(new Client { Nom = "Test", Courriel = "a@a.com", Adresse = "QC" });
        await _context.SaveChangesAsync();

        var result = await _service.GetAllAsync();

        Assert.Single(result);
        Assert.Equal("Test", result[0].Nom);
    }

    [Fact]
    public async Task GetByIdAsync_ClientExists_ShouldReturnDto()
    {
        var client = new Client { Nom = "B", Courriel = "b@b.com", Adresse = "MTL" };
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        var result = await _service.GetByIdAsync(client.ClientId);

        Assert.NotNull(result);
        Assert.Equal("B", result?.Nom);
    }

    [Fact]
    public async Task GetByIdAsync_ClientDoesNotExist_ShouldReturnNull()
    {
        var result = await _service.GetByIdAsync(999);
        Assert.Null(result);
    }
}
