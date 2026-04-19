using MediatR;
using Microsoft.AspNetCore.Mvc;
using MonitorEconomic.Application.Bacen.Parsing;
using MonitorEconomic.Application.Mediator.Bacen.Commands;
using MonitorEconomic.Application.Mediator.Bacen.Queries;

[ApiExplorerSettings(GroupName = "v1")]
[ApiController]
[Route("api/bacen")]
public class BacenController : ControllerBase
{
    private readonly IMediator _mediator;

    public BacenController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBacen( string? serie, string dataInicial, string dataFinal, CancellationToken cancellationToken)
    {
        TryValidarSerie(serie);

        var query = new GetBacenQuery(serie!, dataInicial, dataFinal);
        var resultado = await _mediator.Send(query, cancellationToken);

        if (resultado.Count > 0)
        {
            return Ok(resultado);
        }

        var command = new RefreshBacenCommand(serie!, dataInicial, dataFinal);
        var atualizado = await _mediator.Send(command, cancellationToken);
       
       if (atualizado.Count == 0)
        {
            return NotFound();
        }

        return Ok(atualizado);
    }

    private bool TryValidarSerie(string? serie)
    {
        if (string.IsNullOrWhiteSpace(serie))
        {
            ModelState.AddModelError(nameof(serie), "O parâmetro serie é obrigatório.");
            return false;
        }

        if (!BacenSerieParser.IsValid(serie))
        {
            ModelState.AddModelError(nameof(serie), "O parâmetro serie é inválido.");
            return false;
        }

        return true;
    }
}