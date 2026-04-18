using MediatR;
using Microsoft.AspNetCore.Mvc;
using MonitorEconomic.Application.Bacen.Parsing;
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
    public async Task<IActionResult> GetBacen([FromQuery] string? serie, [FromQuery] string dataInicial, [FromQuery] string dataFinal, CancellationToken cancellationToken)
    {
        if (!TryValidarSerie(serie))
            return ValidationProblem(ModelState);

        var query = new GetBacenQuery(serie!, dataInicial, dataFinal);
        var resultado = await _mediator.Send(query, cancellationToken);

        return Ok(resultado);
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