using MediatR;
using Microsoft.AspNetCore.Mvc;
using MonitorEconomic.Application.Mediator.IPC.Queries;
using MonitorEconomic.Application.Mediator.IPC.Commands;
using MonitorEconomic.Domain.Interfaces.IRepository;


[ApiController]
[Route("api/ipc")]
public class IPCController : ControllerBase
{
    private readonly IMediator _mediator;

    public IPCController(IMediator mediator, IIPCRepository ipcRepository)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> getIPC([FromQuery] string dataInicial, [FromQuery] string dataFinal)
    {
        var query = new GetIPCQuery(dataInicial, dataFinal);

        var resultado = await _mediator.Send(query);

        if (resultado == null)
            return BadRequest("Erro ao buscar dados da API IPC.");

        return Ok(resultado);
    }

    [HttpPost("store")]
    public async Task<IActionResult> storeIPC([FromQuery] string dataInicial, [FromQuery] string dataFinal)
    {
        var command = new CreateIPCCommand(dataInicial, dataFinal);
        var resultado = await _mediator.Send(command);

        if (resultado == null || resultado.Count == 0)
            return BadRequest("Erro ao obter e salvar dados IPC. Verifique datas e conexão à API externa.");

        return Ok(resultado);
    }
    [HttpGet("db")]
    public async Task<IActionResult> getIPCFromDatabase()
    {
        var query = new GetAllIPCQuery();
        var registros = await _mediator.Send(query);

        if (registros == null || registros.Count == 0)
            return NotFound("Nenhum registro de IPC encontrado no banco.");

        return Ok(registros);
    }}


// camada de apresentação vai conhecer apenas a IOC, vai receber o json, transformar ele em um DTO, já vai vir na mesma assinatura, no momento que passar para o application ainda vai ser um DTO, o adapter vai transformar esse DTO em uma entidade do domínio, e na application vai ter uma injeção do medieitor como 