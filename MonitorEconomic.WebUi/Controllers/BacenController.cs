using MediatR;
using Microsoft.AspNetCore.Mvc;
using MonitorEconomic.Application.Mediator.Bacen.Commands;
using MonitorEconomic.Application.Mediator.Bacen.Queries;
using MonitorEconomic.Domain.Enums;

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
    public async Task<IActionResult> GetBacen(BacenSerie serie, string dataInicial, string dataFinal, CancellationToken cancellationToken)
    {
        var query = new GetBacenQuery(serie, dataInicial, dataFinal);
        var resultado = await _mediator.Send(query, cancellationToken);

        if (resultado.Count > 0)
        {
            return Ok(resultado);
        }

        var command = new RefreshBacenCommand(serie, dataInicial, dataFinal);
        var atualizado = await _mediator.Send(command, cancellationToken);
       
       if (atualizado.Count == 0)
        {
            return NotFound();
        }

        return Ok(atualizado);
    }
}