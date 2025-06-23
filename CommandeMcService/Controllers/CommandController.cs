using CommandeMcService.Models;
using CommandeMcService.Services;
using Microsoft.AspNetCore.Mvc;

namespace CommandeMcService.Controllers;

[ApiController]
[Route("api/v1/commandes")]
public class CommandeController : ControllerBase
{
    private readonly ICommandeService _service;
    private readonly ILogger<CommandeController> _logger;

    public CommandeController(ICommandeService service, ILogger<CommandeController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost("valider")]
    public async Task<IActionResult> ValiderCommande([FromBody] CommandeValidationDto dto)
    {
        _logger.LogInformation("Validation de commande pour client {ClientId}", dto.ClientId);

        var success = await _service.ValiderCommandeAsync(dto);
        if (!success)
        {
            _logger.LogWarning("Échec de la validation de la commande pour client {ClientId}", dto.ClientId);
            return BadRequest("Commande invalide (stock insuffisant ou données incorrectes).");
        }

        _logger.LogInformation("Commande validée avec succès pour client {ClientId}", dto.ClientId);
        return Ok("Commande validée avec succès.");
    }
}
