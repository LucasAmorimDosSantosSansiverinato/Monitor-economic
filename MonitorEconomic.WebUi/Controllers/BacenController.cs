using MediatR;
using Microsoft.AspNetCore.Mvc;
using MonitorEconomic.Application.Mediator.Bacen.Commands;
using MonitorEconomic.Application.Mediator.Bacen.Queries;
using MonitorEconomic.Domain.Enums;
using System.Globalization;

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
        var datas = ParseDateRange(dataInicial, dataFinal);
        DateTime dataInicialConvertida = datas.DataInicial;
        DateTime dataFinalConvertida = datas.DataFinal;

        var query = new GetBacenQuery(serie, dataInicialConvertida, dataFinalConvertida);
        var resultado = await _mediator.Send(query, cancellationToken);

        if (resultado.Count > 0)
        {
            return Ok(resultado);
        }

        RefreshBacenCommand command = new RefreshBacenCommand(serie, dataInicialConvertida, dataFinalConvertida);
        var atualizado = await _mediator.Send(command, cancellationToken);
       
       if (atualizado.Count == 0)
        {
            return NotFound();
        }

        return Ok(atualizado);
    }

    private static (DateTime DataInicial, DateTime DataFinal) ParseDateRange(string dataInicial, string dataFinal)
    {
        if (!DateTime.TryParseExact(dataInicial, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataInicialConvertida))
        {
            throw new ArgumentException("data Inicial deve estar com formato em dd/MM/yyyy", nameof(dataInicial));
        }

        if (!DateTime.TryParseExact(dataFinal, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataFinalConvertida))
        {
            throw new ArgumentException("data Final deve estar com formato em dd/MM/yyyy", nameof(dataFinal));
        }

        if (dataInicialConvertida > dataFinalConvertida)
        {
            throw new ArgumentException("data Inicial não pode ser maior que data Final", nameof(dataInicial));
        }

        return (dataInicialConvertida, dataFinalConvertida);
    }
}