using MediatR;
using Microsoft.AspNetCore.Mvc;
using MonitorEconomic.Application.Mediator.Bacen.Queries;
using MonitorEconomic.Application.Mediator.Bacen.Commands;
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
    public async Task<IActionResult> GetBacen([FromQuery] BacenSerie? serie, [FromQuery] string dataInicial, [FromQuery] string dataFinal, CancellationToken cancellationToken)
    {
        if (!TryValidarSerie(serie, out var serieSelecionada))
            return ValidationProblem(ModelState);

        var query = new GetBacenQuery(serieSelecionada, dataInicial, dataFinal);
        var resultado = await _mediator.Send(query, cancellationToken);

        return Ok(resultado);
    }

    [HttpPost("store")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StoreBacen([FromQuery] BacenSerie? serie, [FromQuery] string dataInicial, [FromQuery] string dataFinal, CancellationToken cancellationToken)
    {
        if (!TryValidarSerie(serie, out var serieSelecionada))
            return ValidationProblem(ModelState);

        var command = new CreateBacenCommand(serieSelecionada, dataInicial, dataFinal);
        var resultado = await _mediator.Send(command, cancellationToken);

        return Ok(resultado);
    }

    [HttpGet("db")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBacenFromDatabase(CancellationToken cancellationToken)
    {
        var query = new GetAllBacenQuery();
        var registros = await _mediator.Send(query, cancellationToken);

        if (registros == null || registros.Count == 0)
            return NotFound("Nenhum registro do Bacen encontrado no banco.");

        return Ok(registros);
    }

    private bool TryValidarSerie(BacenSerie? serie, out BacenSerie serieSelecionada)
    {
        if (!serie.HasValue)
        {
            ModelState.AddModelError(nameof(serie), "O parâmetro serie é obrigatório.");
            serieSelecionada = default;
            return false;
        }

        if (!Enum.IsDefined(serie.Value))
        {
            ModelState.AddModelError(nameof(serie), "O parâmetro serie é inválido.");
            serieSelecionada = default;
            return false;
        }

        serieSelecionada = serie.Value;
        return true;
    }
}