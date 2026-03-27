using MediatR;
using Microsoft.AspNetCore.Mvc;
using MonitorEconomic.Application.Mediator.IPC.Queries;
using MonitorEconomic.Application.Mediator.IPC.Commands;

[ApiExplorerSettings(GroupName = "v1")]
[ApiController]
[Route("api/ipc")]
public class IPCController : ControllerBase
{
    private readonly IMediator _mediator;

    public IPCController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> getIPC([FromQuery] string dataInicial, [FromQuery] string dataFinal)
    {
        var query = new GetIPCQuery(dataInicial, dataFinal);

        var resultado = await _mediator.Send(query);

        if (resultado == null)
            return BadRequest("Erro ao buscar dados da API IPC.");

        return Ok(resultado);
    }

    [HttpPost("store")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> storeIPC([FromQuery] string dataInicial, [FromQuery] string dataFinal)
    {
        var command = new CreateIPCCommand(dataInicial, dataFinal);
        var resultado = await _mediator.Send(command);

        if (resultado == null || resultado.Count == 0)
            return BadRequest("Erro ao obter e salvar dados IPC. Verifique datas e conexão à API externa.");

        return Ok(resultado);
    }

    [HttpGet("db")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> getIPCFromDatabase()
    {
        var query = new GetAllIPCQuery();
        var registros = await _mediator.Send(query);

        if (registros == null || registros.Count == 0)
            return NotFound("Nenhum registro de IPC encontrado no banco.");

        return Ok(registros);
    }
}