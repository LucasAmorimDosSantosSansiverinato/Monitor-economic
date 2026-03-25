using MediatR;
using Microsoft.AspNetCore.Mvc;
using MonitorEconomic.Application.Mediator.IPC.Queries;


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
    public async Task<IActionResult> getIPC(string dataInicial, string dataFinal)
    {
        var query = new GetIPCQuery(dataInicial, dataFinal);

        var resultado = await _mediator.Send(query);

        if (resultado == null)
            return BadRequest("Erro ao buscar dados da API IPC.");

        return Ok(resultado);
    }
}


// camada de apresentação vai conhecer apenas a IOC, vai receber o json, transformar ele em um DTO, já vai vir na mesma assinatura, no momento que passar para o application ainda vai ser um DTO, o adapter vai transformar esse DTO em uma entidade do domínio, e na application vai ter uma injeção do medieitor como 