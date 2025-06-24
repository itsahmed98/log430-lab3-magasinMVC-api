using System.Net;
using System.Net.Http.Json;
using ClientMcService.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class ClientControllerIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ClientControllerIntegrationTest(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_Client_ShouldReturnCreated()
    {
        var dto = new CreateClientDto
        {
            Nom = "Toto",
            Courriel = "toto@toto.com",
            Adresse = "QC"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/clients", dto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var returned = await response.Content.ReadFromJsonAsync<ClientDto>();
        Assert.NotNull(returned);
        Assert.Equal("Toto", returned!.Nom);
    }
}
