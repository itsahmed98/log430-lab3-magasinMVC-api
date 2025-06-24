using ClientMcService.Controllers;
using ClientMcService.Models;
using ClientMcService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

public class ClientControllerTest
{
    private readonly Mock<ILogger<ClientController>> _loggerMock = new();
    private readonly Mock<IClientService> _serviceMock = new();

    [Fact]
    public async Task GetAll_ShouldReturnOkWithClients()
    {
        var clients = new List<ClientDto> { new() { ClientId = 1, Nom = "Jean" } };
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(clients);
        var controller = new ClientController(_loggerMock.Object, _serviceMock.Object);

        var result = await controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedClients = Assert.IsType<List<ClientDto>>(okResult.Value);
        Assert.Single(returnedClients);
    }

    [Fact]
    public async Task GetOne_ExistingClient_ShouldReturnOk()
    {
        var dto = new ClientDto { ClientId = 1, Nom = "Test" };
        _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(dto);
        var controller = new ClientController(_loggerMock.Object, _serviceMock.Object);

        var result = await controller.GetOne(1);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task GetOne_NonExistingClient_ShouldReturnNotFound()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((ClientDto?)null);
        var controller = new ClientController(_loggerMock.Object, _serviceMock.Object);

        var result = await controller.GetOne(99);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_ShouldReturnCreated()
    {
        var input = new CreateClientDto { Nom = "Jean", Courriel = "a@a.com", Adresse = "QC" };
        var output = new ClientDto { ClientId = 123, Nom = "Jean", Courriel = "a@a.com", Adresse = "QC" };

        _serviceMock.Setup(s => s.CreateAsync(input)).ReturnsAsync(output);
        var controller = new ClientController(_loggerMock.Object, _serviceMock.Object);

        var result = await controller.Create(input);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdClient = Assert.IsType<ClientDto>(created.Value);
        Assert.Equal(123, createdClient.ClientId);
    }
}
